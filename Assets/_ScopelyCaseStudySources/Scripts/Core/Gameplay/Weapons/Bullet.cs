using System;
using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Characters;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem;
using UnityEngine;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public class Bullet : MonoBehaviour, ILateTickable, IDisposable
    {
        public event Action<Bullet> BulletDisposed;

        [SerializeField] private TrailRenderer _particle;

        private IEnemyControllerSystem _enemyControllerSystem;
        private CharacterView _firingCharacter;

        private float _damage;
        private float _lifetime;
        private float _speed;
        private Vector3 _direction;

        private List<Effect> _additionalEffects;

        private float _currentLifetime;

        private Action<Bullet> _onDispose;

        private GameSession _gameSession;

        public void Initialize(
            GameSession session,
            CharacterView firingCharacter,
            Vector3 direction,
            BulletData bulletData,
            Action<Bullet> onDispose)
        {
            _gameSession = session;

            _enemyControllerSystem = _gameSession.GetSystem<IEnemyControllerSystem>();
            _firingCharacter = firingCharacter;

            _direction = direction;

            _lifetime = bulletData.BulletLifetime;
            _currentLifetime = 0;

            _speed = bulletData.BulletTravelSpeed;
            _damage = bulletData.Damage;

            _onDispose = onDispose;

            transform.LookAt(transform.position + direction);
            _particle.Clear();

            _additionalEffects = bulletData.AdditionalEffects;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(bulletData.Color, 0.0f), new GradientColorKey(bulletData.Color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(bulletData.Color.a, 0.0f), new GradientAlphaKey(bulletData.Color.a, 1.0f) }
            );
            _particle.colorGradient = gradient;
        }

        public void Dispose()
        {
            BulletDisposed?.Invoke(this);
            _particle.Clear();
            _onDispose?.Invoke(this);
        }

        public void LateTick()
        {
            transform.position += _direction * _speed * Time.deltaTime;
            _currentLifetime += Time.deltaTime;

            if (_currentLifetime > _lifetime)
            {
                Dispose();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out EnemyView enemyView) && enemyView != _firingCharacter)
            {
                Damage(_enemyControllerSystem.GetEnemyFromView(enemyView));
            }

            if (other.TryGetComponent(out CharacterView characterView) && characterView == _gameSession.Base.View)
            {
                Damage(_gameSession.Base);
            }
        }

        private void Damage(ICharacter character)
        {
            character.TakeDamage(_damage);
            if (_additionalEffects != null)
            {
                foreach (var effect in _additionalEffects)
                {
                    character.GetModified(effect);
                }
            }

            Dispose();
        }
    }
}
