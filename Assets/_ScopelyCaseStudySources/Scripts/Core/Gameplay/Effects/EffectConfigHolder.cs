using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Effects
{
    [CreateAssetMenu(fileName = "EffectConfigHolder", menuName = "ScopelyCaseStudy/Effect/EffectConfigHolder", order = 0)]
    public class EffectConfigHolder : ScriptableObject
    {
        public List<EffectTypeHolder> Effects;
    }

    [Serializable]
    public struct EffectTypeHolder
    {
        public EffectType EffectType;
        public List<Effect> Effects;
    }
}
