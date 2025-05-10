using System;
using System.Collections.Generic;
using System.Linq;
using CerberusFramework.Core.Managers.Inventory;

namespace CerberusFramework.Utilities.Transaction
{
    public class TransactionController<T> : IDisposable
    {
        private readonly Dictionary<long, TransactionToken<T>> _commitedTransactions = new Dictionary<long, TransactionToken<T>>();
        private long _lastUniqueId;

        private Func<T, bool> _canCommitTransaction;
        private Action<TransactionToken<T>> _commitTransaction;
        private Action<TransactionToken<T>> _rollbackTransaction;
        private Action<TransactionToken<T>> _submitTransaction;

        public void Initialize(
            Func<T, bool> canCommitTransaction,
            Action<TransactionToken<T>> commitTransaction,
            Action<TransactionToken<T>> submitTransaction,
            Action<TransactionToken<T>> rollbackTransaction)
        {
            _canCommitTransaction = canCommitTransaction;
            _commitTransaction = commitTransaction;
            _submitTransaction = submitTransaction;
            _rollbackTransaction = rollbackTransaction;

            _lastUniqueId = 0;
        }

        public void Dispose()
        {
            _canCommitTransaction = null;
            _commitTransaction = null;
            _submitTransaction = null;
            _rollbackTransaction = null;

            _commitedTransactions.Clear();
        }

        public TransactionToken<T> Commit(T data)
        {
            TransactionToken<T> token = null;
            if (!_canCommitTransaction(data))
            {
                return null;
            }

            token = new TransactionToken<T>(data, _lastUniqueId++);
            _commitedTransactions.Add(token.Id, token);
            _commitTransaction(token);

            return token;
        }

        public bool Submit(TransactionToken<T> token)
        {
            var isValid = _commitedTransactions.Remove(token.Id);
            if (isValid)
            {
                _submitTransaction(token);
            }

            return isValid;
        }

        public void SubmitAllCommitted(ResourceKeys resourceType)
        {
            var tokens = _commitedTransactions.Values.Where(
                ct => ct.Data is InventoryTransactionData data && data.ResourceData.Type == resourceType).ToList();

            foreach (var token in tokens)
            {
                Submit(token);
            }
        }

        public bool Rollback(TransactionToken<T> token)
        {
            var isValid = _commitedTransactions.Remove(token.Id);
            if (isValid)
            {
                _rollbackTransaction(token);
            }

            return isValid;
        }

        public List<T> GetCommittedData()
        {
            return _commitedTransactions.Values.Select(ct => ct.Data).ToList();
        }

        public int GetCommitCount(Predicate<TransactionToken<T>> filter)
        {
            var count = 0;
            foreach (var pair in _commitedTransactions)
            {
                if (filter(pair.Value))
                {
                    count++;
                }
            }

            return count;
        }
    }
}