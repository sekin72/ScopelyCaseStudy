using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using CerberusFramework.Utilities.Logging;
using UnityEngine;
using UnityEngine.Profiling;

namespace CerberusFramework.Utilities
{
    public static class PerformanceUtils
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(PerformanceUtils));
        private static bool _loggedFormatString;

        private static readonly Dictionary<string, double> TotalTimeSpentOnMarker = new Dictionary<string, double>();

        public static void LogFormatString()
        {
            if (_loggedFormatString)
            {
                return;
            }

            _loggedFormatString = true;

            Logger.Debug("Name, Duration, Starting Frame, Finished Frame");
        }

        public static IDisposable Stopwatch(string markerName)
        {
            return new StopwatchDisposable(markerName);
        }

        public static IDisposable CFStopwatch(string markerName)
        {
            return new CFStopwatchDisposable(markerName);
        }

        public class StopwatchDisposable : IDisposable
        {
            private readonly int _startingFrame;

            private readonly string _name;

            private readonly Stopwatch _stopwatch = new Stopwatch();
            private double _timeSpent;

            public StopwatchDisposable(string debugName)
            {
                LogFormatString();

                _name = debugName;

                TotalTimeSpentOnMarker[_name] = 0;

                _startingFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;

                _stopwatch.Start();
            }

            protected virtual string ProfilerSampleName => "**Stopwatch duration**";

            public void Dispose()
            {
                _timeSpent += _stopwatch.Elapsed.TotalMilliseconds;
                TotalTimeSpentOnMarker[_name] += _timeSpent;
                _stopwatch.Stop();

                Profiler.BeginSample(ProfilerSampleName);

                var frameCount = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
                Logger.Debug(
                    $"{_name}, {_timeSpent:N2} ms, {_startingFrame} F, {frameCount} F"
                );

                Profiler.EndSample();
            }
        }

        public class CFStopwatchDisposable : StopwatchDisposable
        {
            public CFStopwatchDisposable(string debugName) : base(debugName)
            {
            }

            protected override string ProfilerSampleName => "**CF Stopwatch duration**";
        }
    }
}