using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using VContainer.Unity;

namespace ScopelyCaseStudy
{
    public abstract class Character<TD, TV> : Controller<TD, TV>, ICharacter, ILateTickable
        where TV : CharacterView
        where TD : CharacterData
    {
        protected MovementComponent MovementComponent { get; set; }
        protected LifeComponent LifeComponent { get; set; }
        protected AttackComponent AttackComponent { get; set; }

        protected GameSession GameSession;

        private List<Effect> _ongoingEffects = new List<Effect>();

        public void SetSession(GameSession gameSession)
        {
            GameSession = gameSession;
        }

        protected override UniTask Initialize(CancellationToken cancellationToken)
        {
            _ongoingEffects.Clear();

            MovementComponent = View.MovementComponent;
            LifeComponent = View.LifeComponent;
            AttackComponent = View.AttackComponent;

            if (LifeComponent != null)
            {
                LifeComponent.Died += OnDeath;
            }

            return UniTask.CompletedTask;
        }

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
        }

        protected override void Dispose()
        {
            if (LifeComponent != null)
            {
                LifeComponent.Died -= OnDeath;
            }
        }

        public abstract void OnDeath();

        public bool IsAlive()
        {
            return LifeComponent == null || LifeComponent.IsAlive();
        }

        public void TakeDamage(float damage)
        {
            LifeComponent?.TakeDamage(damage);
        }

        public virtual void LateTick()
        {
            MovementComponent?.LateTick();
            LifeComponent?.LateTick();
            AttackComponent?.LateTick();
        }

        public void GetModified(Effect effect)
        {
            if (_ongoingEffects.Any((x) => x.EffectType == effect.EffectType))
            {
                return;
            }

            _ongoingEffects.Add(effect);
            switch (effect.AffectedComponent)
            {
                case AffectedComponentType.MovementComponent:
                    MovementComponent?.GetModified(effect);
                    break;
                case AffectedComponentType.LifeComponent:
                    LifeComponent?.GetModified(effect);
                    break;
                case AffectedComponentType.AttackComponent:
                    AttackComponent?.GetModified(effect);
                    break;
            }

            RevertEffect(effect).Forget();
        }

        private async UniTask RevertEffect(Effect effect)
        {
            await effect.ApplyEffect(GameSession.CancellationTokenSource.Token);

            switch (effect.AffectedComponent)
            {
                case AffectedComponentType.MovementComponent:
                    MovementComponent?.ReverseEffect(effect);
                    break;
                case AffectedComponentType.LifeComponent:
                    LifeComponent?.ReverseEffect(effect);
                    break;
                case AffectedComponentType.AttackComponent:
                    AttackComponent?.ReverseEffect(effect);
                    break;
            }

            _ongoingEffects.Remove(effect);
        }
    }
}
