using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Extensions;
using UnityEngine;
using VContainer.Unity;

namespace CerberusFramework.Core.Managers
{
    public abstract class Manager : IAsyncStartable, IManager, IDisposable
    {
        private readonly UnityStopwatch _preInitializeStopwatch = new UnityStopwatch();
        private readonly UnityStopwatch _initializeStopwatch = new UnityStopwatch();
        private readonly UnityStopwatch _dependencyStopwatch = new UnityStopwatch();
        private readonly UnityStopwatch _optionalDependencyStopwatch = new UnityStopwatch();
        private readonly UnityStopwatch _postInitializeStopwatch = new UnityStopwatch();
        private readonly UnityStopwatch _readyStopWatch = new UnityStopwatch();

        private IPublisher<ManagerStateChangedEvent> _managerStateChangedEventPublisher;

        private IDisposable _messageSubscription;
        private IDisposable _dependencySubscription;

        private List<IManager> _waitingDependencies;
        private List<IManager> _optionalWaitingDependencies;

        protected CancellationToken DisposeToken;
        private CancellationTokenSource _disposeTokenSource;

        static Manager()
        {
            Application.quitting += () => IsApplicationQuiting = true;
        }

        protected static bool IsApplicationQuiting { get; private set; }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            _disposeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            DisposeToken = _disposeTokenSource.Token;

            _managerStateChangedEventPublisher = GlobalMessagePipe.GetPublisher<ManagerStateChangedEvent>();

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<ApplicationPauseEvent>().Subscribe(OnApplicationPause).AddTo(bagBuilder);
            GlobalMessagePipe.GetSubscriber<ApplicationQuitEvent>().Subscribe(OnApplicationQuit).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            PreInitializeWrapper();

            if (ManagerState.IsTerminal())
            {
                return UniTask.CompletedTask;
            }

            CheckDependencies();
            return UniTask.CompletedTask;
        }

        public virtual void Dispose()
        {
            Save();

            _messageSubscription?.Dispose();
            _dependencySubscription?.Dispose();

            _disposeTokenSource?.Cancel();
            _disposeTokenSource?.Dispose();
        }

        public ManagerState ManagerState { get; private set; }
        public abstract bool IsCore { get; }

        private void PreInitializeWrapper()
        {
            _preInitializeStopwatch.Start();
            PreInitialize();
            _preInitializeStopwatch.Stop();
        }

        private void CheckDependencies()
        {
            _waitingDependencies = GetDependencies();
            _dependencyStopwatch.Start();

            if (_waitingDependencies != null && !_waitingDependencies.AreAllReady())
            {
                SetState(ManagerState.WaitingDependencies);

                var bagBuilder = DisposableBag.CreateBuilder();
                GlobalMessagePipe.GetSubscriber<ManagerStateChangedEvent>().Subscribe(OnDependencyWait).AddTo(bagBuilder);
                _dependencySubscription = bagBuilder.Build();
                return;
            }

            _dependencyStopwatch.Stop();
            CheckOptionalDependencies();
        }

        private void OnDependencyWait(ManagerStateChangedEvent evt)
        {
            if (!evt.NewState.IsTerminal())
            {
                return;
            }

            if (!_waitingDependencies.AreAllTerminalState())
            {
                return;
            }

            _dependencySubscription.Dispose();
            _dependencyStopwatch.Stop();

            if (!_waitingDependencies.AreAllReady())
            {
                SetState(ManagerState.DependenciesFailed);
                return;
            }

            CheckOptionalDependencies();
        }

        private void CheckOptionalDependencies()
        {
            _optionalWaitingDependencies = GetOptionalDependencies();
            _optionalDependencyStopwatch.Start();

            if (_optionalWaitingDependencies != null && !_optionalWaitingDependencies.AreAllReady())
            {
                SetState(ManagerState.WaitingDependencies);

                var bagBuilder = DisposableBag.CreateBuilder();
                GlobalMessagePipe.GetSubscriber<ManagerStateChangedEvent>().Subscribe(OnOptionalDependencyWait).AddTo(bagBuilder);
                _dependencySubscription = bagBuilder.Build();
                return;
            }

            _optionalDependencyStopwatch.Stop();
            InitializeWrapper(DisposeToken).Forget();
        }

        private void OnOptionalDependencyWait(ManagerStateChangedEvent evt)
        {
            if (!evt.NewState.IsTerminal())
            {
                return;
            }

            if (!_optionalWaitingDependencies.AreAllTerminalState())
            {
                return;
            }

            _dependencySubscription.Dispose();
            _optionalDependencyStopwatch.Stop();

            InitializeWrapper(DisposeToken).Forget();
        }

        private async UniTask InitializeWrapper(CancellationToken cancellationToken)
        {
            _initializeStopwatch.Start();
            _readyStopWatch.Start();

            SetState(ManagerState.Initializing);
            await Initialize(cancellationToken);
            _initializeStopwatch.Stop();

            PostInitializeWrapper();
        }

        private void PostInitializeWrapper()
        {
            _postInitializeStopwatch.Start();
            PostInitialize();
            _postInitializeStopwatch.Stop();
        }

        public bool IsReady()
        {
            return ManagerState.IsReady();
        }

        public bool IsTerminalState()
        {
            return ManagerState.IsTerminal();
        }

        public static string GetLoadingMetricsHeader()
        {
            return string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                "Name",
                "PreInitDur", //Elapsed duration of PreInitialize
                "DepDur", //Elapsed duration of dependency wait
                "OptDepDur", //Elapsed duration of optional dependency wait
                "InitDur", //Elapsed duration of initialization
                "PostInitDur", //Elapsed duration of post initialization
                "ReadyDur", //Elapsed duration of waiting from Initialize until SetReady
                "ReadyTS", //SetReady timestamp
                "PreInitEF", //Elapsed frame count of PreInitialize
                "DepEF", //Elapsed frame count of dependency wait
                "OptDepEF", //Elapsed frame count of optional dependency wait
                "InitEF", //Elapsed frame count of initialization
                "PostInitEF", //Elapsed frame count of post initialization
                "ReadyEF", //Elapsed frame count of waiting from Initialize until SetReady
                "ReadyFS" //SetReady frame stamp
            );
        }

        public string GetLoadingMetrics()
        {
            return
                string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7:F3},{8},{9},{10},{11},{12},{13},{14}",
                    GetType().Name,
                    _preInitializeStopwatch.ElapsedTimeMilliseconds.ToString(),
                    _dependencyStopwatch.ElapsedTimeMilliseconds.ToString(),
                    _optionalDependencyStopwatch.ElapsedTimeMilliseconds.ToString(),
                    _initializeStopwatch.ElapsedTimeMilliseconds.ToString(),
                    _postInitializeStopwatch.ElapsedTimeMilliseconds.ToString(),
                    _readyStopWatch.ElapsedTimeMilliseconds.ToString(),
                    _readyStopWatch.EndTime,
                    _preInitializeStopwatch.ElapsedFrameCount.ToString(),
                    _dependencyStopwatch.ElapsedFrameCount.ToString(),
                    _optionalDependencyStopwatch.ElapsedFrameCount.ToString(),
                    _initializeStopwatch.ElapsedFrameCount.ToString(),
                    _postInitializeStopwatch.ElapsedFrameCount.ToString(),
                    _readyStopWatch.ElapsedFrameCount.ToString(),
                    _readyStopWatch.EndFrame.ToString()
                );
        }

        protected void SetReady()
        {
            _readyStopWatch.Stop();
            SetState(ManagerState.Ready);
            Save();
        }

        protected abstract UniTask Initialize(CancellationToken disposeToken);

        protected void SetState(ManagerState state)
        {
            if (ManagerState.IsTerminal() && ManagerState != state)
            {
                throw new InvalidOperationException($"Cannot change terminal state from {ManagerState} to {state}");
            }

            var oldState = ManagerState;
            ManagerState = state;

            _managerStateChangedEventPublisher.Publish(new ManagerStateChangedEvent(this, oldState, state));
        }

        protected virtual List<IManager> GetDependencies()
        {
            return null;
        }

        protected virtual List<IManager> GetOptionalDependencies()
        {
            return null;
        }

        protected virtual void PreInitialize()
        {
        }

        protected virtual void PostInitialize()
        {
        }

        protected virtual void OnApplicationPause(ApplicationPauseEvent evt)
        {
        }

        protected virtual void OnApplicationQuit(ApplicationQuitEvent evt)
        {
        }

        #region Data

        protected virtual void SaveData()
        {
        }

        protected virtual void LoadData()
        {
        }

        public void Save()
        {
            if (!IsReady())
            {
                return;
            }

            SaveData();
        }

        #endregion Data
    }
}