using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Asset;
using VContainer;

namespace CerberusFramework.Core.Managers.Pool
{
    public sealed class PoolWarmUpManager : Manager
    {
        private CancellationTokenSource _oldCancellationTokenSource;

        public override bool IsCore => true;

        private AddressableManager _addressableManager;
        private PoolManager _poolManager;

        public bool LevelWarmUpCompleted { get; private set; }
        private bool _warmUpStarted;

        [Inject]
        private void Inject(AddressableManager addressableManager, PoolManager poolManager)
        {
            _addressableManager = addressableManager;
            _poolManager = poolManager;
        }

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>()
            {
                _addressableManager,
                _poolManager
            };
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            LevelWarmUpCompleted = false;
            _warmUpStarted = false;

            InitializeLevelItemPools(disposeToken);
            SetReady();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            if (_oldCancellationTokenSource != null)
            {
                _oldCancellationTokenSource.Cancel();
                _oldCancellationTokenSource.Dispose();
                _oldCancellationTokenSource = null;
            }

            base.Dispose();
        }

        private void InitializeLevelItemPools(CancellationToken cancellationToken)
        {
            var poolDataHolder = _addressableManager.PoolDataHolder;

            Queue<PoolWarmJob> jobs = new Queue<PoolWarmJob>();

            foreach (var enumType in Enum.GetValues(typeof(PoolKeys)))
            {
                var key = (PoolKeys)enumType;
                if (key == PoolKeys.None)
                {
                    continue;
                }

                var def = poolDataHolder.GetPoolDefinition(key);

                if (key == PoolKeys.None)
                {
                    continue;
                }

                var job = new PoolWarmJob(
                    key,
                    cancellationToken,
                    def.DefaultCapacity
                );
                jobs.Enqueue(job);
            }

            StartWarmUpJobs(jobs, cancellationToken).Forget();
            _warmUpStarted = true;
        }

        private async UniTask StartWarmUpJobs(Queue<PoolWarmJob> jobs, CancellationToken cancellationToken)
        {
            while (jobs.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var currentJob = jobs.Dequeue();
                    await _poolManager.WarmupPool(
                        currentJob.PoolKey,
                        currentJob.DefaultCapacity,
                        currentJob.CancellationToken
                    );
                }
                catch (OperationCanceledException)
                {
                    //Ignored
                }
            }

            LevelWarmUpCompleted = true;
        }

        public async UniTask StartRemainingJobsForceful(CancellationToken cancellationToken)
        {
            if (!_warmUpStarted)
            {
                await UniTask.WaitUntil(() => _warmUpStarted, cancellationToken: cancellationToken);
            }

            if (_oldCancellationTokenSource != null)
            {
                _oldCancellationTokenSource.Cancel();
                _oldCancellationTokenSource.Dispose();
            }

            _oldCancellationTokenSource = null;

            LevelWarmUpCompleted = true;
        }

        private sealed class PoolWarmJob
        {
            public readonly CancellationToken CancellationToken;
            public readonly int DefaultCapacity;
            public readonly PoolKeys PoolKey;

            public PoolWarmJob(
                PoolKeys poolKey,
                CancellationToken cancellationToken,
                int defaultCapacity)
            {
                PoolKey = poolKey;
                CancellationToken = cancellationToken;
                DefaultCapacity = defaultCapacity;
            }
        }
    }
}