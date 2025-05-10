using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers;
using UnityEngine.Pool;

namespace CerberusFramework.Utilities.Extensions
{
    public static class IManagerExtensions
    {
        public static async UniTask<bool> WhenAllReady(this IList<IManager> managers, CancellationToken cancellation)
        {
            var dependencies = ListPool<IManager>.Get();
            dependencies.AddRange(managers);

            while (dependencies.Count > 0)
            {
                for (var i = dependencies.Count - 1; i >= 0; i--)
                {
                    var dependency = dependencies[i];
                    if (!dependency.ManagerState.IsTerminal())
                    {
                        continue;
                    }

                    dependencies.RemoveAt(i);
                    if (!dependency.ManagerState.IsReady())
                    {
                        continue;
                    }

                    ListPool<IManager>.Release(dependencies);
                    return false;
                }

                await UniTask.Yield(cancellation);
            }

            ListPool<IManager>.Release(dependencies);
            return true;
        }

        public static async UniTask<ManagerState[]> WhenAllInTerminalState(this IList<IManager> managers,
            CancellationToken cancellation)
        {
            while (true)
            {
                var isAllTerminal = true;
                foreach (var manager in managers)
                {
                    if (manager.ManagerState.IsTerminal())
                    {
                        continue;
                    }

                    isAllTerminal = false;
                    break;
                }

                if (isAllTerminal)
                {
                    break;
                }

                await UniTask.Yield(cancellation);
            }

            var results = new ManagerState[managers.Count];
            for (var i = 0; i < managers.Count; i++)
            {
                results[i] = managers[i].ManagerState;
            }

            return results;
        }

        public static bool AreAllTerminalState(this IList<IManager> managers)
        {
            foreach (var manager in managers)
            {
                if (!manager.ManagerState.IsTerminal())
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAllReady(this IList<IManager> managers)
        {
            foreach (var manager in managers)
            {
                if (!manager.ManagerState.IsReady())
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreAllCoreReady(this IList<IManager> managers)
        {
            foreach (var manager in managers)
            {
                if (!manager.IsCore)
                {
                    continue;
                }

                if (!manager.ManagerState.IsReady())
                {
                    return false;
                }
            }

            return true;
        }
    }
}