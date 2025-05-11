using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.ScoreSystem
{
    [CreateAssetMenu(fileName = "ScoreSystem", menuName = "ScopelyCaseStudy/Systems/ScoreSystem", order = 2)]
    public class ScoreSystem : GameSystem, IScoreSystem
    {
        public override Type RegisterType => typeof(IScoreSystem);

        private IDisposable _messageSubscription;
        private IPublisher<ScoreChangedEvent> _scoreChangedEventPublisher;
        private IPublisher<GoldChangedEvent> _goldChangedEventPublisher;

        private int _score;
        private int _gold;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<EnemyKilledEvent>().Subscribe(OnEnemyKilledEvent).AddTo(bagBuilder);
            GlobalMessagePipe.GetSubscriber<TurretSoldEvent>().Subscribe(OnTurretSoldEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _scoreChangedEventPublisher = GlobalMessagePipe.GetPublisher<ScoreChangedEvent>();
            _goldChangedEventPublisher = GlobalMessagePipe.GetPublisher<GoldChangedEvent>();

            _score = Session.GameSessionSaveStorage.CurrentScore;
            _gold = Session.GameSessionSaveStorage.Gold;
        }

        public override void Activate()
        {
            _scoreChangedEventPublisher.Publish(new ScoreChangedEvent(_score));
            _goldChangedEventPublisher.Publish(new GoldChangedEvent(_gold));
        }

        public override void Deactivate()
        {
            _messageSubscription?.Dispose();
        }

        public override void Dispose()
        {
        }

        private void OnEnemyKilledEvent(EnemyKilledEvent evt)
        {
            _score += evt.IncreaseScoreAmount;
            _gold += evt.IncreaseGoldAmount;

            _scoreChangedEventPublisher.Publish(new ScoreChangedEvent(_score));
            _goldChangedEventPublisher.Publish(new GoldChangedEvent(_gold));

            Session.GameSessionSaveStorage.CurrentScore = _score;
            Session.GameSessionSaveStorage.HighScore = _score > Session.GameSessionSaveStorage.HighScore ? _score : Session.GameSessionSaveStorage.HighScore;
            Session.GameSessionSaveStorage.Gold = _gold;

            Session.SaveGameSessionStorage();
        }

        private void OnTurretSoldEvent(TurretSoldEvent evt)
        {
            _gold += evt.Cost;

            _goldChangedEventPublisher.Publish(new GoldChangedEvent(_gold));

            Session.GameSessionSaveStorage.Gold = _gold;

            Session.SaveGameSessionStorage();
        }
    }
}
