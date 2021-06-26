using System;
using MClient.Utils;

namespace MClient.Render
{
    public class MAnimation
    {

        private Transition transition;
        protected int duration;
        private bool isForward = true;
        private bool inProgress;
        protected double progress;
        static readonly double InertiaCost = MMathUtils.Cbrt(0.5) / 0.5;

        public MAnimation(Transition trans, int dur, bool isForwards = true, bool startRunning = false)
        {
            transition = trans;
            duration = dur;
            isForward = isForwards;
            inProgress = startRunning;
            progress = isForwards ? 0 : 1;
        }

        public MAnimation Start()
        {
            inProgress = true;
            return this;
        }

        public MAnimation Stop()
        {
            inProgress = false;
            return this;
        }

        public MAnimation Cancel()
        {
            return Reset().Stop();
        }

        public MAnimation Reset()
        {
            progress = isForward ? 0 : 1;
            return this;
        }

        public MAnimation Reverse()
        {
            isForward = false;
            return Start();
        }

        public MAnimation Forward()
        {
            isForward = true;
            return Start();
        }

        public MAnimation SetTransition(Transition trans)
        {
            transition = trans;
            return this;
        }

        public MAnimation SetDuration(int dur)
        {
            duration = dur;
            return this;
        }

        public MAnimation SetProgress(double progressIn)
        {
            progress = progressIn;
            return this;
        }

        public bool IsRunning()
        {
            return inProgress;
        }

        public double Get()
        {
            Increment(1);
            return Translate(progress);
        }

        public double Get(double low, double high)
        {
            return ((high - low) * Get() + low);
        }

        private void Increment(int ticks)
        {
            if (!inProgress) return;
            if (!isForward) ticks *= -1;
            progress += ticks / (double) duration;

            if (progress > 1)
            {
                progress = 1;
                inProgress = false;
            }
            else if (progress < 0)
            {
                progress = 0;
                inProgress = false;
            }
        }

        protected double Translate(double progress)
        {
            switch (transition)
            {
                case Transition.Curve:
                    return progress * progress;
                case Transition.SteepCurve:
                    return Math.Pow(progress, 3);
                case Transition.BezierCurve:
                    return Math.Pow(-1 + Math.Sqrt(-progress + 1), 2);
                case Transition.InverseCurve:
                    return -Math.Pow(progress - 1, 2) + 1;
                case Transition.InverseSteepCurve:
                    return Math.Pow(progress - 1, 3) + 1;
                case Transition.Rubber:
                    double trans = -Math.Sin(10.0 * progress) / (10.0 * progress) + 1;
                    return trans > 0 ? trans : 0;
                case Transition.Inertia:
                    return MMathUtils.Cbrt(progress - 0.5) / InertiaCost + 0.5;
                case Transition.Instant:
                    return Math.Round(progress);
            }

            return progress;
        }
    }
    
    public enum Transition {
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
