using System;

namespace EasyScheduler.Tiny.Core
{
    internal class FetchCycle
    {
        private readonly TimeSpan _Range;

        public FetchCycle(DateTime minNextFireTime, TimeSpan range)
        {
            MinNextFireTime = minNextFireTime;
            _Range = range;
        }

        public DateTime MinNextFireTime { get; private set; }
        public DateTime MaxNextFireTime { get { return MinNextFireTime + _Range; } }

        public void PushForward(TimeSpan runnerCycleIncrement)
        {
            MinNextFireTime = MinNextFireTime + runnerCycleIncrement;

        }

        public TimeSpan GetMinTimeSpanToBePushForward(TimeSpan runnerCycleIncrement, DateTime minCurrentFireTime)
        {
            if (minCurrentFireTime < MinNextFireTime + runnerCycleIncrement)
            {
                return minCurrentFireTime - MinNextFireTime;
            }
            return runnerCycleIncrement;
        }
    }
}