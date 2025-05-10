using System.Threading;
using Cysharp.Threading.Tasks;

namespace CerberusFramework.Core.MVC
{
    public interface IController : ILifecycle
    {
        Data Data { get; }
        View View { get; }

        UniTask InitializeController(Data data, View view, CancellationToken cancellationToken);

        void ActivateController();

        void DeactivateController();

        void DisposeController();
    }
}