using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay;

namespace CFGameClient.Core.Gameplay.Systems
{
    public interface IGameSystem : IDisposable
    {
        Type RegisterType { get; }

        UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken);

        void Activate();

        void Deactivate();
    }
}