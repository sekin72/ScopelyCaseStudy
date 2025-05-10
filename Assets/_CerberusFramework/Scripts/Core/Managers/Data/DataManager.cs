using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.Data.Syncers;
using CerberusFramework.Utilities.Logging;
using UnityEngine;

namespace CerberusFramework.Core.Managers.Data
{
    public class DataManager : Manager, IDataManager
    {
        private const float SaveInterval = 30.0f;

        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(DataManager));
        protected string SaveKey = "Save";
        protected bool IsSaveDirty;
        private float _lastSaveTime;

        private ILocalSyncer<SaveStorage> _localSyncer;
        private SaveStorage _saveStorage;

        public override bool IsCore => true;

        public virtual T Load<T>() where T : class, IStorage, new()
        {
            return !IsReady() ? throw new InvalidOperationException("Trying to load data before DataManager is ready") : _saveStorage.Get<T>();
        }

        public virtual void Save<T>(T data) where T : class, IStorage, new()
        {
            if (!IsReady())
            {
                throw new InvalidOperationException("Trying to save data before DataManager is ready");
            }

            _saveStorage.Set(data);
            IsSaveDirty = true;
        }

        public void ForceSave()
        {
            Logger.Debug("Force Save");
            SaveAll();
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            _localSyncer = new LocalStorageSyncer<SaveStorage>(SaveKey, PlayerPrefs.GetInt("PlayerID").ToString());

            _saveStorage = await _localSyncer.Load(disposeToken);

            StartAutoSavingJob(disposeToken).Forget();
            SetReady();
        }

        protected async UniTaskVoid StartAutoSavingJob(CancellationToken cancellationToken)
        {
            Logger.Debug("[SaveJob] Start auto save Job");
            while (true)
            {
                await UniTask.WaitUntil(
                    () => IsSaveDirty && _lastSaveTime + SaveInterval < Time.realtimeSinceStartup,
                    PlayerLoopTiming.PostLateUpdate,
                    cancellationToken
                );

                Logger.Debug("[SaveJob] Auto save triggered");
                SaveAll();
            }
        }

        private void SaveAll()
        {
            if (!IsSaveDirty)
            {
                return;
            }

            _lastSaveTime = Time.realtimeSinceStartup;
            IsSaveDirty = false;

            SaveLocal();
        }

        protected override void OnApplicationPause(ApplicationPauseEvent evt)
        {
            if (!IsReady())
            {
                return;
            }

            ForceSave();
        }

        protected virtual void SaveLocal()
        {
            _localSyncer.Save(_saveStorage);
        }

        public override void Dispose()
        {
            if (!IsReady())
            {
                return;
            }

            ForceSave();

            base.Dispose();
        }
    }
}