using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CerberusFramework.Utilities.Logging
{
    public static class CerberusLogger
    {
        public static readonly ICerberusLogger DefaultLogger;

        private static readonly Dictionary<string, WeakReference<ICerberusLogger>> Loggers;

        static CerberusLogger()
        {
            Loggers = new Dictionary<string, WeakReference<ICerberusLogger>>();
            DefaultLogger = GetLogger("default");
        }

        public static ICerberusLogger GetLogger(string label)
        {
            if (label == null)
            {
                return DefaultLogger;
            }

            ICerberusLogger logger = null;
            if (Loggers.TryGetValue(label, out var weakRefToLogger))
            {
                weakRefToLogger.TryGetTarget(out logger);
            }

            return logger ?? CreateLogger(label);
        }

        public static ICerberusLogger GetLogger(Type typeLabel)
        {
            return GetLogger(typeLabel.Name);
        }

        private static ICerberusLogger CreateLogger(string label)
        {
            var labellingLogger = new UnityBridge(Debug.unityLogger, label);
            Loggers.Add(label, new WeakReference<ICerberusLogger>(labellingLogger));
            return labellingLogger;
        }
    }

    public sealed class UnityBridge : ICerberusLogger
    {
        private readonly ILogger _baseLogger;
        private readonly string _label;

        public UnityBridge(ILogger baseLogger, string label)
        {
            _baseLogger = baseLogger;
            _label = "[" + label + "]";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Debug(string message, string caller = null, int line = 0)
        {
            _baseLogger.Log(LogType.Log, $"{_label}-[{caller}:{line}] {message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warn(string message, string caller = null, int line = 0)
        {
            _baseLogger.Log(LogType.Warning, $"{_label}-[{caller}:{line}] {message}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string message, string caller = null, int line = 0)
        {
            _baseLogger.Log(LogType.Error, $"{_label}-[{caller}:{line}] {message}");
        }
    }
}