using System;
using MClient.Core.Utils;

namespace MClient.RenderSystem
{
    /// <summary>
    /// Class for animating values with more interesting curves between start and end values.
    /// </summary>
    public class MAnimation
    {

        private MTransition _transition;
        private int _duration;
        private bool _isForward;
        private bool _inProgress;
        private double _progress;
        private static readonly double InertiaCost = MMathUtils.CubeRoot(0.5) / 0.5;

        /// <summary>
        /// Creates a new animation instance
        /// </summary>
        /// <param name="trans">The type of transition</param>
        /// <param name="dur">The duration of the transition, in "ticks"</param>
        /// <param name="isForwards">Whether to run the animation forwards or backwards</param>
        /// <param name="startRunning">Whether to immediately start running the animation or not</param>
        public MAnimation(MTransition trans, int dur, bool isForwards = true, bool startRunning = false)
        {
            _transition = trans;
            _duration = dur;
            _isForward = isForwards;
            _inProgress = startRunning;
            _progress = isForwards ? 0 : 1;
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Start()
        {
            _inProgress = true;
            return this;
        }

        /// <summary>
        /// Stops/Pauses the animation. Does not reset any values.
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Stop()
        {
            _inProgress = false;
            return this;
        }

        /// <summary>
        /// Cancels the animation. Resets all values.
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Cancel()
        {
            return Reset().Stop();
        }

        /// <summary>
        /// Resets all values. Does not stop the animation.
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Reset()
        {
            _progress = _isForward ? 0 : 1;
            return this;
        }

        /// <summary>
        /// Sets the animation direction to backwards.
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Reverse()
        {
            _isForward = false;
            return Start();
        }
        
        /// <summary>
        /// Sets the animation direction to forwards.
        /// </summary>
        /// <returns>This</returns>
        public MAnimation Forward()
        {
            _isForward = true;
            return Start();
        }

        /// <summary>
        /// Sets the transition type to use
        /// </summary>
        /// <param name="trans">The transition type to use</param>
        /// <returns>This</returns>
        public MAnimation SetTransition(MTransition trans)
        {
            _transition = trans;
            return this;
        }

        /// <summary>
        /// Sets the duration to use
        /// </summary>
        /// <param name="dur">The duration to use</param>
        /// <returns>This</returns>
        public MAnimation SetDuration(int dur)
        {
            _duration = dur;
            return this;
        }

        /// <summary>
        /// Sets the current progress
        /// </summary>
        /// <param name="progressIn">The progress value</param>
        /// <returns>This</returns>
        public MAnimation SetProgress(double progressIn)
        {
            _progress = progressIn;
            return this;
        }

        /// <summary>
        /// Gets whether the animation is running
        /// </summary>
        /// <returns>Whether the animation is running</returns>
        public bool IsRunning()
        {
            return _inProgress;
        }

        /// <summary>
        /// Increments the animation by one "tick" and returns the value for this step.
        /// </summary>
        /// <returns>The value for this step</returns>
        public double Get()
        {
            Increment(1);
            return Translate(_progress);
        }

        /// <summary>
        /// Increments the animation by one "tick" and returns the value for this step, remapped to the given range.
        /// </summary>
        /// <param name="low">The minimum of the range to remap to</param>
        /// <param name="high">The maximum of the range to remap to</param>
        /// <returns>The value for this step, remapped to the given range.</returns>
        public double Get(double low, double high)
        {
            return ((high - low) * Get() + low);
        }

        private void Increment(int ticks)
        {
            if (!_inProgress) return;
            if (!_isForward) ticks *= -1;
            _progress += ticks / (double) _duration;

            if (_progress > 1)
            {
                _progress = 1;
                _inProgress = false;
            }
            else if (_progress < 0)
            {
                _progress = 0;
                _inProgress = false;
            }
        }

        private double Translate(double progress)
        {
            switch (_transition)
            {
                case MTransition.Curve:
                    return progress * progress;
                case MTransition.SteepCurve:
                    return Math.Pow(progress, 3);
                case MTransition.BezierCurve:
                    return Math.Pow(-1 + Math.Sqrt(-progress + 1), 2);
                case MTransition.InverseCurve:
                    return -Math.Pow(progress - 1, 2) + 1;
                case MTransition.InverseSteepCurve:
                    return Math.Pow(progress - 1, 3) + 1;
                case MTransition.Rubber:
                    double trans = -Math.Sin(10.0 * progress) / (10.0 * progress) + 1;
                    return trans > 0 ? trans : 0;
                case MTransition.Inertia:
                    return MMathUtils.CubeRoot(progress - 0.5) / InertiaCost + 0.5;
                case MTransition.Instant:
                    return Math.Round(progress);
                case MTransition.Linear:
                    return progress;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public enum MTransition {
        Linear,
        Curve,
        SteepCurve,
        BezierCurve,
        InverseCurve,
        InverseSteepCurve,
        Rubber,
        Inertia,
        Instant,
    }
}
