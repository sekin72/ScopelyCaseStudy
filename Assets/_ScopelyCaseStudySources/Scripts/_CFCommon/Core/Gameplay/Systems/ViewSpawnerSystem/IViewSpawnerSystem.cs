using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.MVC;

namespace CFGameClient.Core.Gameplay.Systems.ViewSpawner
{
    public interface IViewSpawnerSystem : IGameSystem
    {
        public T Spawn<T>(PoolKeys poolKey) where T : View;

        public void Despawn<T>(PoolKeys poolKey, T t) where T : View;
    }
}