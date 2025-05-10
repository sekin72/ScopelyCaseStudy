using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using CerberusFramework.Core.Constants;
using UnityEngine;

namespace CerberusFramework.Core.UI.Components
{
    public class TextTween : MonoBehaviour
    {
        public CFText Text;
        public Animator Animator;
        [SerializeField] protected Canvas Canvas;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isComplete;

        private bool _isRunning;

        public async UniTask Play(string text, Transform root, Vector3 offset, CancellationToken cancellationToken, bool overrideSorting = false,
            int sortingOrder = 100, int desiredWidth = 380)
        {
            var rectTransform = GetComponent<RectTransform>();
            var r = rectTransform.sizeDelta;
            r.x = desiredWidth;
            rectTransform.sizeDelta = r;

            _isRunning = true;
            _isComplete = false;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Text.Text = text;

            var tweenTransform = transform;
            tweenTransform.SetParent(root);
            tweenTransform.localPosition = offset;
            tweenTransform.localScale = Vector3.one;

            Canvas.overrideSorting = overrideSorting;
            Canvas.sortingOrder = sortingOrder;

            Animator.SetTrigger(AnimationConstants.Play);

            await UniTask.WaitUntil(() => _isComplete, cancellationToken: _cancellationTokenSource.Token);

            _cancellationTokenSource.Dispose();

            _isRunning = false;
            _isComplete = false;
        }

        [UsedImplicitly]
        public void OnAnimationComplete()
        {
            _isComplete = true;
        }

        public void Cancel()
        {
            if (!_isRunning)
            {
                return;
            }

            if (_isComplete || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            Animator.SetTrigger(AnimationConstants.Reset);
            _cancellationTokenSource.Cancel();
        }
    }
}