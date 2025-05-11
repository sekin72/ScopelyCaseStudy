using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Utilities.Logging;

namespace CerberusFramework.Core.MVC
{
    public abstract class Controller<TD, TV> : IController
        where TV : View
        where TD : Data
    {
        protected static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(Controller<TD, TV>));

        public TD Data { get; private set; }
        public TV View { get; private set; }
        Data IController.Data => Data;
        View IController.View => View;

        public async UniTask InitializeController(Data data, View view, CancellationToken cancellationToken)
        {
            if (Data != null && Data.IsInitialized)
            {
                Logger.Warn($"Trying to initialize {View.name}, but it's already initialized");
                return;
            }

            Data = data as TD;
            View = view as TV;
            View.SetData(Data);

            Data.IsInitialized = true;
            Data.IsActivated = false;
            Data.IsDeactivated = false;
            Data.IsDisposed = false;

            await Initialize(cancellationToken);

            await View.Initialize(cancellationToken);
        }

        public void ActivateController()
        {
            if (Data.IsActivated)
            {
                Logger.Warn($"Trying to activate {View.name}, but it's already activated");
                return;
            }

            Data.IsActivated = true;

            Activate();

            View.Activate();
        }

        public void DeactivateController()
        {
            if (Data.IsDeactivated)
            {
                Logger.Warn($"Trying to deactivate {View.name}, but it's already deactivated");
                return;
            }

            Data.IsDeactivated = true;
            Data.IsActivated = false;

            Deactivate();

            View.Deactivate();
        }

        public void DisposeController()
        {
            if (Data.IsDisposed)
            {
                Logger.Warn($"Trying to dispose {View.name}, but it's already disposed");
                return;
            }

            Dispose();

            View.Dispose();

            Data.IsInitialized = false;
            Data.IsActivated = false;
            Data.IsDeactivated = false;
            Data.IsDisposed = true;
        }

        protected abstract UniTask Initialize(CancellationToken cancellationToken);

        protected abstract void Activate();

        protected abstract void Deactivate();

        protected abstract void Dispose();

        UniTask ILifecycle.Initialize(CancellationToken cancellationToken) => Initialize(cancellationToken);

        void ILifecycle.Activate() => Activate();

        void ILifecycle.Deactivate() => Deactivate();

        void ILifecycle.Dispose() => Dispose();
    }
}