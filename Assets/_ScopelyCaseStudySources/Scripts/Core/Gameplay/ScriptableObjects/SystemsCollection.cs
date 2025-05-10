using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Systems;
using UnityEngine;

namespace CFGameClient.Core.Gameplay
{
    [CreateAssetMenu(fileName = "SystemsCollection", menuName = "CerberusFramework/SystemsCollection", order = 1)]
    public class SystemsCollection : ScriptableObject
    {
        public List<GameSystem> Systems;
    }
}