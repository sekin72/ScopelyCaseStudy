using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using CerberusFramework.Core.Managers.Vibration;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CerberusFramework.Core.UI.FlyTweens.Inventory
{
    public abstract class InventoryFlyTweenView : FlyTweenView
    {
        private VibrationManager _vibrationManager;

        public Image Icon;
        public Canvas Canvas;

        public AnimationCurve XEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public AnimationCurve YEaseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public float ContactDistancePercentage = .9f;
        public float ScaleDistancePercentage = .2f;
        public float StartScale = 1f;
        public float EndScale = .5f;

        private int _iconOriginalSortingOrder;

        private Vector3 _targetWorldPosition;
        private Sequence _sequence;
        private Tween _scaleTween;
        private bool _scaleTweenStarted;
        private bool _onContactInvoked;
        private bool _finished;

        [Inject]
        public void Inject(VibrationManager vibrationManager)
        {
            _vibrationManager = vibrationManager;
        }

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            transform.localScale = Vector3.one;

            var data = Data as InventoryFlyTweenData;

            _iconOriginalSortingOrder = Canvas.sortingOrder;
            Canvas.sortingOrder += data.AdditionalViewOrder;

            _targetWorldPosition = data.Target.position;

            transform.position = data.Parent.position;
            _scaleTweenStarted = true;
        }

        public override void Activate()
        {
            base.Activate();

            transform.localScale = Vector3.one * StartScale;
            _scaleTweenStarted = false;
            _onContactInvoked = false;
            _finished = false;
        }

        public override async UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            var duration = DurationInSeconds;

            var time = 0f;
            _sequence = DOTween.Sequence()
                .Append(transform.DOMoveY(_targetWorldPosition.y, duration).SetEase(YEaseCurve))
                .OnUpdate(
                    () =>
                    {
                        time += Time.deltaTime;
                        if (!_scaleTweenStarted &&
                            time / duration >= ScaleDistancePercentage)
                        {
                            _scaleTweenStarted = true;
                            _scaleTween = transform.DOScale(Vector3.one * EndScale, duration - time);
                        }

                        if (!_onContactInvoked &&
                            time / duration >= ContactDistancePercentage)
                        {
                            _onContactInvoked = true;
                        }
                    }
                )
                .OnComplete(
                    () =>
                    {
                        _vibrationManager.Vibrate(VibrationType.LightImpact);
                        _finished = true;
                    }
                )
                .Insert(0, transform.DOMoveX(_targetWorldPosition.x, duration).SetEase(XEaseCurve));

            await UniTask.WaitUntil(() => _finished, cancellationToken: cancellationToken);
        }

        public override void Dispose()
        {
            Canvas.sortingOrder = _iconOriginalSortingOrder;

            _scaleTween?.Kill();
            _sequence?.Kill(!_onContactInvoked);

            base.Dispose();
        }
    }
}