using System;

namespace EasyScheduler.Tiny
{
    public class SchedulerSetting
    {
        private readonly TimeSpan _FetchRange;
        private readonly TimeSpan _SchedulerIdleTime;
        private readonly TimeSpan _RunnerCycleIncrement;

        public TimeSpan FetchRange { get { return _FetchRange; } }
        public TimeSpan SchedulerIdleTime { get { return _SchedulerIdleTime; } }
        public TimeSpan RunnerCycleIncrement { get { return _RunnerCycleIncrement; }}

        public SchedulerSetting(TimeSpan fetchRange, TimeSpan schedulerIdleTime, TimeSpan runnerCycleIncrement)
        {
            _FetchRange = fetchRange;
            _SchedulerIdleTime = schedulerIdleTime;
            _RunnerCycleIncrement = runnerCycleIncrement;
        }

        public static SchedulerSetting Default()
        {
            return new SchedulerSetting(new TimeSpan(0, 5, 0), new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));
        }
    }
}