using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Utilities.Extensions;
using CerberusFramework.Utilities.Transaction;
using VContainer;

namespace CerberusFramework.Core.Managers.Inventory
{
    public class InventoryManager : Manager
    {
        private readonly DataManager _dataManager;

        private IPublisher<InventoryChangedEvent> _inventoryChangedPublisher;
        private IPublisher<InventoryRollbackEvent> _inventoryRollbackPublisher;

        private InventoryStorage _storage;

        private TransactionController<InventoryTransactionData> _transactionController;

        public override bool IsCore => true;

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>
            {
                _dataManager
            };
        }

        [Inject]
        public InventoryManager(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            _transactionController = new TransactionController<InventoryTransactionData>();
            _transactionController.Initialize(OnCanCommit, OnCommit, OnSubmit, OnRollback);

            _inventoryChangedPublisher = GlobalMessagePipe.GetPublisher<InventoryChangedEvent>();
            _inventoryRollbackPublisher = GlobalMessagePipe.GetPublisher<InventoryRollbackEvent>();

            LoadData();

            SetReady();

            var waitingTransactions = _storage.TransactionItems.ToKeyList();
            foreach (var transactionId in waitingTransactions)
            {
                var transaction = _storage.TransactionItems[transactionId];
                ProcessTransaction(transaction);
                _storage.TransactionItems.Remove(transactionId);
            }

            return UniTask.CompletedTask;
        }

        public int GetValue(ResourceKeys type)
        {
            _storage.InventoryItems.TryGetValue((int)type, out var value);
            return value;
        }

        public bool IsValid(List<ResourceData> dataList)
        {
            return dataList.TrueForAll(IsValid);
        }

        public bool IsValid(ResourceData data)
        {
            return data.Value != 0 && GetValue(data.Type) + data.Value >= 0;
        }

        private void ProcessTransaction(InventoryTransactionData transaction)
        {
            UpdateInventoryItem(transaction.ResourceData, transaction.Reason);
        }

        private bool UpdateInventoryItem(ResourceData data, string reason)
        {
            var typeId = (int)data.Type;

            _storage.InventoryItems.TryGetValue(typeId, out var initialValue);

            var finalValue = initialValue + data.Value;

            _storage.InventoryItems[typeId] = finalValue;

            Save();

            if (finalValue != initialValue)
            {
                _inventoryChangedPublisher.Publish(new InventoryChangedEvent(data.Type));
            }

            return true;
        }

        protected override void LoadData()
        {
            _storage = _dataManager.Load<InventoryStorage>();
            if (_storage != null)
            {
                return;
            }

            _storage = new InventoryStorage();
            Save();
        }

        protected override void SaveData()
        {
            _dataManager.Save(_storage);
        }

        #region Transaction

        public List<InventoryTransactionToken> Commit(List<ResourceData> resources, string reason)
        {
            var tokens = new List<InventoryTransactionToken>();

            foreach (var resource in resources)
            {
                tokens.Add(Commit(resource, reason));
            }

            return tokens;
        }

        public InventoryTransactionToken Commit(ResourceData data, string reason)
        {
            var transactionData = new InventoryTransactionData(data, reason);

            return new InventoryTransactionToken(_transactionController.Commit(transactionData));
        }

        public bool Submit(InventoryTransactionToken token)
        {
            return _transactionController.Submit(token);
        }

        public void SubmitAll(ResourceKeys type)
        {
            _transactionController.SubmitAllCommitted(type);
        }

        public bool ProcessAtomic(ResourceData data, string reason)
        {
            return IsValid(data) && UpdateInventoryItem(data, reason);
        }

        public bool Rollback(InventoryTransactionToken token)
        {
            return _transactionController.Rollback(token);
        }

        private bool OnCanCommit(InventoryTransactionData data)
        {
            return data.ResourceData.Value <= 0 ? throw new InvalidOperationException("Consumption operations must be done trough ProcessAtomic!") :
                string.IsNullOrEmpty(data.Reason) ? throw new InvalidOperationException("Reason must be provided!") : GetValue(data.ResourceData.Type) + data.ResourceData.Value >= 0;
        }

        private void OnCommit(TransactionToken<InventoryTransactionData> token)
        {
            _storage.TransactionItems[token.Id] = token.Data;
            Save();
        }

        private void OnSubmit(TransactionToken<InventoryTransactionData> token)
        {
            _storage.TransactionItems.Remove(token.Id);
            ProcessTransaction(token.Data);
        }

        private void OnRollback(TransactionToken<InventoryTransactionData> token)
        {
            _inventoryRollbackPublisher.Publish(new InventoryRollbackEvent(token.Data.ResourceData));
        }

        #endregion Transaction
    }
}