using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CerberusFramework.Core.Managers.Pool
{
    public class GameObjectPoolContainer : IDisposable
    {
        public PoolKeys Key;

        public Action<GameObject> OnGetCallback;
        public Action<GameObject> OnReleaseCallback;
        public ObjectPool<GameObject> Pool;
        public GameObject Prefab;

        public GameObjectPoolContainer(
            PoolKeys key,
            GameObject prefab,
            ObjectPool<GameObject> pool,
            Action<GameObject> onGetCallback,
            Action<GameObject> onReleaseCallback)
        {
            Key = key;
            Prefab = prefab;
            Pool = pool;
            OnGetCallback = onGetCallback;
            OnReleaseCallback = onReleaseCallback;
        }

        public void Dispose()
        {
            Prefab = null;
            OnGetCallback = null;
            OnReleaseCallback = null;
            Pool.Dispose();
        }
    }
}