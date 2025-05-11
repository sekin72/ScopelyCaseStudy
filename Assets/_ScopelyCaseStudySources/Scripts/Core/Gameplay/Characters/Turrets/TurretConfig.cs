using System.Collections;
using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "ScopelyCaseStudy/Data/TurretConfig", order = 3)]
    public class TurretConfig : CharacterConfig
    {
        public List<EffectType> AdditionalEffects;
    }
}
