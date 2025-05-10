using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CerberusFramework.Core.Scenes
{
    public class LoadingSceneController : SceneController
    {
        private GameObject _canvasGroup;

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _canvasGroup ??= UIContainer?.gameObject;
            return base.Activate(cancellationToken);
        }

        public virtual UniTask ShowEffect(CancellationToken cancellationToken)
        {
            _canvasGroup.SetActive(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask HideEffect(CancellationToken cancellationToken)
        {
            _canvasGroup.SetActive(false);
            return UniTask.CompletedTask;
        }
    }
}