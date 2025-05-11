using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.LevelAssets
{
    public class LevelView : View
    {
        public BaseView BaseView;
        public List<SpawnPointView> SpawnPoints;
        public Transform EnemyParent;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
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

#if UNITY_EDITOR
        [Button("Fetch Transforms")]
        public void FetchSpawnPoints()
        {
            BaseView = GetComponentInChildren<BaseView>();

            SpawnPoints = new List<SpawnPointView>();
            foreach (var spawnPoint in GetComponentsInChildren<SpawnPointView>())
            {
                SpawnPoints.Add(spawnPoint);
            }

            EnemyParent = transform.GetChild(transform.childCount - 1);
        }

#endif
    }
}