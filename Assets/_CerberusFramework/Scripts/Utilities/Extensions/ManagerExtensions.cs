using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers;

namespace CerberusFramework.Utilities.Extensions
{
    public static class ManagerExtensions
    {
        public static async UniTask WaitUntilReady(this Manager manager, CancellationToken cancellation)
        {
            await UniTask.WaitUntil(manager.IsReady, cancellationToken: cancellation);
        }

        public static async UniTask<ManagerState> WaitTerminalState(this Manager manager,
            CancellationToken cancellation)
        {
            await UniTask.WaitUntil(manager.IsTerminalState, cancellationToken: cancellation);
            return manager.ManagerState;
        }
    }
}