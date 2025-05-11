using System.Collections.Generic;
using CFGameClient.Core.Gameplay.Systems;
using ScopelyCaseStudy.Core.Gameplay.Characters.Turrets;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.TurretPlacerSystem
{
    public interface ITurretPlacerSystem : IGameSystem
    {
        public List<TurretConfig> TurretConfigs { get; }
    }
}
