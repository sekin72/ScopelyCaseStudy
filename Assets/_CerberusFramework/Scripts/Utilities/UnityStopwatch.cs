using System;
using UnityEngine;

namespace CerberusFramework.Utilities
{
    public class UnityStopwatch
    {
        private bool _isRunning;
        public double StartTime { get; private set; }
        public int StartFrame { get; private set; }
        public double EndTime { get; private set; }
        public int EndFrame { get; private set; }
        public int ElapsedTimeMilliseconds => _isRunning ? -1 : TimeSpan.FromSeconds(EndTime - StartTime).Milliseconds;
        public int ElapsedFrameCount => _isRunning ? -1 : EndFrame - StartFrame;

        public void Start()
        {
            StartTime = Time.realtimeSinceStartupAsDouble;
            StartFrame = Time.frameCount;
            _isRunning = true;
        }

        public void Stop()
        {
            EndTime = Time.realtimeSinceStartupAsDouble;
            EndFrame = Time.frameCount;
            _isRunning = false;
        }
    }
}