using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace CerberusFramework.Utilities
{
    // Used to prevent unintentional overriding of some state by different sources.
    // Would be nice to implement a leak detection with stack trace to keep track of who forgot to restore a state when needed
    // Leak detection example: https://github.com/dotnet/roslyn/blob/main/src/Dependencies/PooledObjects/ObjectPool%601.cs
    public class LockBin : IDisposable
    {
        private readonly List<string> _lockBin = new List<string>();

        public void Dispose()
        {
            _lockBin.Clear();
            StateChanged = null;
        }

        /// <summary>
        ///     Clear all locked entries on the LockBin
        ///     Only to be used on refresh state where the game could get to a frozen state
        /// </summary>
        public void ForceUnlock()
        {
            _lockBin.Clear();
        }

        public event Action<bool> StateChanged;

        public bool Contains(string id)
        {
            foreach (var lockId in _lockBin)
            {
                if (id.Equals(lockId))
                {
                    return true;
                }
            }

            return false;
        }

        public void Increase(string id)
        {
            var oldState = (bool)this;

            _lockBin.Add(id);

            var newState = (bool)this;
            if (oldState != newState)
            {
                StateChanged?.Invoke(newState);
            }
        }

        public void Decrease(string id)
        {
            var oldState = (bool)this;
            if (!_lockBin.Remove(id))
            {
                throw new IndexOutOfRangeException(id + " is not in the LockBin list");
            }

            var newState = (bool)this;
            if (oldState != newState)
            {
                StateChanged?.Invoke(newState);
            }
        }

        public int ToInt()
        {
            return _lockBin.Count;
        }

        public IEnumerable<string> GetLocks()
        {
            return _lockBin;
        }

        public string GetLocksAsString()
        {
            return string.Join(", ", _lockBin);
        }

        public static implicit operator bool(LockBin counter)
        {
            return counter._lockBin.Count > 0;
        }

        public static LockBin Get()
        {
            return GenericPool<LockBin>.Get();
        }

        public static void Release(LockBin lockBin)
        {
            lockBin.Dispose();
            GenericPool<LockBin>.Release(lockBin);
        }

        public static void ReleaseAll<T>(Dictionary<T, LockBin> lockBins)
        {
            foreach (var pair in lockBins)
            {
                Release(pair.Value);
            }

            lockBins.Clear();
        }
    }
}