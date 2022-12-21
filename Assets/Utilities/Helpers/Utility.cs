using System;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Utilities
{
    public static class Utility
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitDelay = new Dictionary<float, WaitForSeconds>();

        /// <summary>
        /// Container that stores/reuses newly created WaitForSeconds.
        /// </summary>
        /// <param name="time">time(s) to wait</param>
        /// <returns>new WaitForSeconds</returns>
        public static WaitForSeconds GetWaitForSeconds(float time)
        {
            if (WaitDelay.TryGetValue(time, out var waitForSeconds)) return waitForSeconds;

            WaitDelay[time] = new WaitForSeconds(time);

            return WaitDelay[time];
        }


        private static TimeSpan _timeSpan;

        /// <summary>
        /// Displays time in 00h:00m:00s format.
        /// </summary>
        public static string TimeFormat(float time)
        {
            _timeSpan = TimeSpan.FromSeconds(time);

            return _timeSpan.Hours < 60
                ? $"{_timeSpan.Minutes}m:{_timeSpan.Seconds}s" :
                _timeSpan.Hours >= 60 ?
                    $"{_timeSpan.Hours}h:{_timeSpan.Minutes}m:{_timeSpan.Seconds}s" :
                    string.Empty;
        }

        public static int GetAnimId(string id)
        {
            return Animator.StringToHash(id);
        }
    }
}