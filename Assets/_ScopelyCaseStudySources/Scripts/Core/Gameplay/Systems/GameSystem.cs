using System;
using System.Threading;
using CFGameClient.Core.Gameplay.Systems;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Systems
{
    public abstract class GameSystem : ScriptableObject, IGameSystem
    {
        protected GameSession Session;

        protected string LockBinKey;
        public abstract Type RegisterType { get; }

        public virtual UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            Session = gameSession;

            LockBinKey = RegisterType.ToString();

            return UniTask.CompletedTask;
        }

        public abstract void Activate();

        public abstract void Deactivate();

        public abstract void Dispose();
    }
}