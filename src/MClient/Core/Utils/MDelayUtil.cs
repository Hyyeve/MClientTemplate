using System.Diagnostics;

namespace MClient.Core.Utils
{
    /// <summary>
    /// Utility for easily checking whether a certain amount of time has passed
    /// </summary>
    public class MDelayUtil
    {
        private readonly Stopwatch _timer = new Stopwatch();
        
        /// <summary>
        /// Resets this DelayUtil
        /// </summary>
        public void Reset()
        {
            _timer.Restart();
        }
        
        /// <summary>
        /// Checks if the given amount of time has elapsed since this method last returned true.
        /// </summary>
        /// <param name="mills">The amount of time, in milliseconds</param>
        /// <returns>True, if the given amount of time has passed since this method last returned true.</returns>
        public bool TimePassed(float mills)
        {
            if (!_timer.IsRunning) _timer.Start();
            if (_timer.ElapsedMilliseconds < mills) return false;
            Reset();
            return true;
        }

        /// <summary>
        /// Checks if the given amount of time has elapsed since this DelayUtil was last reset.
        /// </summary>
        /// <param name="mills">The amount of time, in milliseconds</param>
        /// <returns>True, if the given amount of time has passed since this DelayUtil was last reset.</returns>
        public bool AbsoluteTimePassed(float mills)
        {
            if (!_timer.IsRunning) _timer.Start();
            return !(_timer.ElapsedMilliseconds < mills);
        }
    }
}