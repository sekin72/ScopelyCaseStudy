using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace CerberusFramework.Core.Scenes
{
    public abstract class SceneController : MonoBehaviour
    {
        public Camera SceneCamera;
        [SerializeField] protected EventSystem EventSystem;
        public UIContainer UIContainer;

        [UsedImplicitly]
        public void Awake()
        {
            SetState(SceneManager.GetActiveScene().name == gameObject.scene.name);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            Dispose();
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            await UIContainer.Initialize(cancellationToken);
        }

        public virtual async UniTask Activate(CancellationToken cancellationToken)
        {
            SceneCamera.gameObject.SetActive(true);

            await UIContainer.Activate(cancellationToken);
        }

        public virtual UniTask Deactivate(CancellationToken cancellationToken)
        {
            SceneCamera.gameObject.SetActive(false);

            UIContainer?.Deactivate();

            return UniTask.CompletedTask;
        }

        protected virtual void Dispose()
        {
        }

        private void OnActiveSceneChanged(Scene fromScene, Scene toScene)
        {
            SetState(toScene.name == gameObject.scene.name);
        }

        private void SetState(bool isActive)
        {
            EventSystem?.gameObject.SetActive(isActive);
        }

        public virtual void SceneVisible()
        {
        }

        public virtual void SceneInvisible()
        {
        }

        public virtual void SceneWillBeDeactivated()
        {
        }

        public virtual bool KeepSceneLoaded()
        {
            return true;
        }
    }
}