using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using VContainer;

namespace CerberusFramework.Core.Managers.UI
{
    public class TextTweenManager : Manager
    {
        private readonly List<TextTween> _currentTweens = new List<TextTween>();

        public override bool IsCore => true;

        private PoolManager _poolManager;

        [Inject]
        private void Inject(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            SetReady();
            return UniTask.CompletedTask;
        }

        public async UniTask ShowText(string textToTween, Transform root, Vector3 offset, CancellationToken cancellationToken, bool overrideSorting = false,
            int sortingOrder = 100, int desiredWidth = 380)
        {
            var textTweenGo = _poolManager.GetGameObject(PoolKeys.TextTween);
            var textTween = textTweenGo.GetComponent<TextTween>();
            _currentTweens.Add(textTween);

            try
            {
                await textTween.Play(textToTween, root, offset, cancellationToken, overrideSorting, sortingOrder, desiredWidth);
            }
            finally
            {
                _poolManager.SafeReleaseObject(PoolKeys.TextTween, textTween.gameObject);
            }
        }

        public void KillAll()
        {
            foreach (var tween in _currentTweens)
            {
                tween.Cancel();
            }
        }

        public override void Dispose()
        {
            KillAll();

            base.Dispose();
        }
    }
}