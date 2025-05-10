using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CerberusFramework.Core.Managers;

namespace CerberusFramework.Utilities.Extensions
{
    public static class ManagerStateExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReady(this ManagerState state)
        {
            return state == ManagerState.Ready;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDisabled(this ManagerState state)
        {
            return state == ManagerState.Disabled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFailed(this ManagerState state)
        {
            return
                state == ManagerState.Failed ||
                state == ManagerState.DependenciesFailed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTerminal(this ManagerState state)
        {
            return
                state == ManagerState.Disabled ||
                state == ManagerState.Ready ||
                state == ManagerState.Failed ||
                state == ManagerState.DependenciesFailed;
        }

        public static bool IsAllReady(this IList<ManagerState> states)
        {
            foreach (var state in states)
            {
                if (!state.IsReady())
                {
                    return false;
                }
            }

            return true;
        }
    }
}