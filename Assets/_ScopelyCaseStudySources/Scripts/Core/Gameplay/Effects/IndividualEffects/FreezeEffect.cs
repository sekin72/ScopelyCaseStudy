using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Effects
{
    [CreateAssetMenu(fileName = "FreezeEffect", menuName = "ScopelyCaseStudy/Effect/FreezeEffect", order = 7)]
    public class FreezeEffect : Effect
    {
        public override EffectType EffectType => EffectType.Freeze;
        public override AffectedCharacterType AffectedCharacter => AffectedCharacterType.Enemy;
        public override AffectedComponentType AffectedComponent => AffectedComponentType.MovementComponent;

        public override async UniTask ApplyEffect(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Time), cancellationToken: cancellationToken);
        }
    }
}
