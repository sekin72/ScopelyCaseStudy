using System.Threading;
using Cysharp.Threading.Tasks;

namespace CerberusFramework
{
    public interface ILifecycle
    {
        UniTask Initialize(CancellationToken cancellationToken);

        void Activate();

        void Deactivate();

        void Dispose();
    }

    public interface IGradualLifecycle
        : ILifecycle
    {
        UniTask ActivateGradual(CancellationToken cancellationToken);

        UniTask DeactivateGradual(CancellationToken cancellationToken);
    }
}