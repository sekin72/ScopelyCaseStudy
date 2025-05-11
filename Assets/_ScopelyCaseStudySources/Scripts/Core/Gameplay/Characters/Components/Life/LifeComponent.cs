using System;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public sealed class LifeComponent : Component, ILateTickable
    {
        public event Action Died;

        [SerializeField] private HealthBar _healthBar;

        private float _currentHealth;
        private float _maxHealth;

        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);

            _maxHealth = characterConfig.Health;
            _currentHealth = _maxHealth;
            _healthBar.ForceSetHealth(_currentHealth, _maxHealth);
        }

        public override void Dispose()
        {
            _healthBar.Dispose();
            base.Dispose();
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive())
            {
                return;
            }

            _currentHealth -= damage;

            if (_healthBar != null)
            {
                _healthBar.UpdateHealth(_currentHealth, _maxHealth);
            }

            if (!IsAlive())
            {
                Die();
            }
        }

        private void Die()
        {
            Died?.Invoke();
        }

        public void LateTick()
        {
            _healthBar.transform.LookAt(GameSession.LevelCamera.transform.position);
        }

        public override void GetModified(Effect effect)
        {
        }

        public override void ReverseEffect(Effect effect)
        {
        }
    }
}
