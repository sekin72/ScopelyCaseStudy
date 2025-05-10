using System;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.MVC;
using ScopelyCaseStudy.Core.Gameplay.Systems;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Gameplay.Systems.ViewSpawner
{
    [CreateAssetMenu(fileName = "ViewSpawnerSystem", menuName = "CerberusFramework/Systems/ViewSpawnerSystem", order = 2)]
    public sealed class ViewSpawnerSystem : GameSystem, IViewSpawnerSystem
    {
        public override Type RegisterType => typeof(IViewSpawnerSystem);

        private PoolManager _poolManager;

        [Inject]
        private void Inject(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
        }

        public T Spawn<T>(PoolKeys poolKey) where T : View
        {
            var view = _poolManager.GetGameObject(poolKey).GetComponent<T>();

            view.transform.position = Vector3.zero;
            view.transform.rotation = Quaternion.identity;
            view.transform.localScale = Vector3.one;

            return view;
        }

        public void Despawn<T>(PoolKeys poolKey, T t) where T : View
        {
            _poolManager.SafeReleaseObject(poolKey, t.gameObject);
        }
    }
}