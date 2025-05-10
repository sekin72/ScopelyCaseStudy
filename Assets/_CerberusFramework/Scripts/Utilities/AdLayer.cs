using System;
using System.Collections.Generic;

namespace CerberusFramework.Utilities
{
    public static class AdLayer
    {
        public static readonly RatingSystem RatingSystem;
    }

    public class RatingSystem
    {
        protected RatingSystem()
        {
        }

        public void Rate()
        {
        }
    }

    public enum AdResult
    {
        None,
        Close,
        Error,
        Reward
    }

    public static class AdUtility
    {
        public delegate void RewardedAdAction(string unitId, string from, AdResult result);

        public static event Action InterstitialStarted;

        public static event Action RewardedStarted;

        public static event Action InterstitialClosed;

        public static event Action RewardedClosed;

        public static event RewardedAdAction OnRewardedAdWatched;

        public static bool HasRewardedVideo => true;

        public static bool ShowRewardedAd(string from)
        {
            OnRewardedAdWatched?.Invoke("", from, AdResult.Reward);
            return true;
        }

        public static bool ShowInterstitialAd(string target = "")
        {
            return true;
        }
    }

    public static class AnalyticsUtility
    {
        public static void SendEventWithParameters(string eventName, Dictionary<string, object> parameters,
            bool withGameProgressTracing = true)
        {
        }

        public static void SendEvent(string eventName,
            bool withGameProgressTracing = true)
        {
        }
    }
}

namespace CerberusFramework.Utilities.RCManager
{
    public static class RCManager
    {
        public static object UsingConfigId => "local";

        public static event Action<bool> FetchingCompleted;
    }
}