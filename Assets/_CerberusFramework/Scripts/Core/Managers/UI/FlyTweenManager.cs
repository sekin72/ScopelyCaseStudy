using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.FlyTweens;
using VContainer;
using VContainer.Unity;

namespace CerberusFramework.Core.Managers.UI
{
    public class FlyTweenManager : Manager
    {
        private PoolManager _poolManager;

        private readonly List<IFlyTween> _flyTweens = new List<IFlyTween>();

        private IObjectResolver _resolver;

        public override bool IsCore => true;

        [Inject]
        private void Inject(IObjectResolver resolver, PoolManager poolManager)
        {
            _resolver = resolver;
            _poolManager = poolManager;
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            SetReady();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            KillAll();
            base.Dispose();
        }

        private void KillAll()
        {
            foreach (var tween in _flyTweens)
            {
                Release(tween);
            }

            _flyTweens.Clear();
        }

        public async UniTask FlyTween<TP, TD, TV>(
            TD data,
            CancellationToken cancellationToken)
            where TP : FlyTween<TD, TV>, new()
            where TD : FlyTweenData
            where TV : FlyTweenView
        {
            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, DisposeToken
            );

            TP flyTween = null;
            try
            {
                flyTween = await Create<TP, TD, TV>(data, cancellationToken);
                flyTween.ActivateController();
                await flyTween.ActivateGradual(linkedCancellationTokenSource.Token);
                data.OnComplete?.Invoke();
            }
            finally
            {
                if (flyTween?.View?.gameObject != null)
                {
                    await Release(flyTween, linkedCancellationTokenSource.Token);
                }

                linkedCancellationTokenSource.Dispose();
            }
        }

        public bool IsThereAnyTweenRunning()
        {
            return _flyTweens.Count > 0;
        }

        private async UniTask<TP> Create<TP, TD, TV>(
            TD data,
            CancellationToken cancellationToken)
            where TP : FlyTween<TD, TV>
            where TD : FlyTweenData
            where TV : FlyTweenView
        {
            var flyTween = _resolver.Resolve<TP>();
            _flyTweens.Add(flyTween);

            var flyTweenViewGo = _poolManager.GetGameObject(data.PoolKey);
            var flyTweenView = flyTweenViewGo.GetComponent<TV>();
            flyTweenView.transform.SetParent(data.UIContainer.transform);

            _resolver.InjectGameObject(flyTweenViewGo);

            await flyTween.InitializeController(data, flyTweenView, cancellationToken);

            return flyTween;
        }

        private void Release(IFlyTween flyTween)
        {
            _flyTweens.Remove(flyTween);

            flyTween.DeactivateController();
            flyTween.DisposeController();

            _poolManager.SafeReleaseObject(flyTween.Data.PoolKey, flyTween.View.gameObject);
        }

        private async UniTask Release(IFlyTween flyTween, CancellationToken cancellationToken)
        {
            _flyTweens.Remove(flyTween);

            await flyTween.DeactivateGradual(cancellationToken);
            flyTween.DeactivateController();
            flyTween.DisposeController();

            _poolManager.SafeReleaseObject(flyTween.Data.PoolKey, flyTween.View.gameObject);
        }
    }
}