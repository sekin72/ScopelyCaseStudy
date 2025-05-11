using System;
using CerberusFramework.Core.Scenes;
using CFGameClient.Core.Gameplay.Systems;
using Cysharp.Threading.Tasks;

namespace CFGameClient.Core.Gameplay
{
    public interface IGameSession
    {
        public UniTask Initialize(SceneController levelSceneController);
        public void Dispose();

        public T GetSystem<T>() where T : IGameSystem;
    }
}