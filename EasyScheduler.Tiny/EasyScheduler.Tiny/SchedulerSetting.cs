using System;

namespace EasyScheduler.Tiny
{
    public class SchedulerSetting
    {
        private readonly TimeSpan _FetchTriggersRange;
        private readonly TimeSpan _SchedulerIdleTime;
        private readonly TimeSpan _RunnerCycleIncrement;

        public TimeSpan FetchTriggersRange { get { return _FetchTriggersRange; } }
        public TimeSpan SchedulerIdleTime { get { return _SchedulerIdleTime; } }
        public TimeSpan RunnerCycleIncrement { get { return _RunnerCycleIncrement; }}

        public SchedulerSetting(TimeSpan fetchTriggersRange, TimeSpan schedulerIdleTime, TimeSpan runnerCycleIncrement)
        {
            _FetchTriggersRange = fetchTriggersRange;
            _SchedulerIdleTime = schedulerIdleTime;
            _RunnerCycleIncrement = runnerCycleIncrement;
        }

        public static SchedulerSetting Default()
        {
            return new SchedulerSetting(new TimeSpan(0, 5, 0), new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));
        }
    }
}