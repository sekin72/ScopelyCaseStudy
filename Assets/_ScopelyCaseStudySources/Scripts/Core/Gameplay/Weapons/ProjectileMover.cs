using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public class ProjectileMover : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _speed = 0.1f;

        private Tween _moveTween;

        public async UniTask Move(Vector3 target, CancellationToken cancellationToken)
        {
            _particleSystem.Play();
            _moveTween = transform.DOMove(target, _speed).SetEase(Ease.Linear).SetLink(gameObject);
            await _moveTween.ToUniTask(TweenCancelBehaviour.KillWithCompleteCallback, cancellationToken: cancellationToken);
            _particleSystem.Stop();
        }
    }
}
