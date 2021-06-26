using System.Diagnostics;

namespace MClient.Utils
{
    public class MDelayUtil
    {
        private readonly Stopwatch timer = new Stopwatch();
        public void Reset()
        {
            timer.Restart();
        }
        
        public bool TimePassed(float mills)
        {
            if (!timer.IsRunning) timer.Start();
            if (timer.ElapsedMilliseconds < mills) return false;
            Reset();
            return true;
        }

        public bool AbsoluteTimePassed(float mills)
        {
            if (!timer.IsRunning) timer.Start();
            return !(timer.ElapsedMilliseconds < mills);
        }
    }
}