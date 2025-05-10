using CFGameClient.Core.Gameplay.Systems;
using ScopelyCaseStudy.Core.Gameplay.LevelAssets;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.LevelControllerSystem
{
    public interface ILevelControllerSystem : IGameSystem
    {
        public LevelView Level { get; }
    }
}
