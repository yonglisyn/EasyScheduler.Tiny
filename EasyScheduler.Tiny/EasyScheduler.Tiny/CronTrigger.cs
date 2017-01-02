using System;
using NCrontab.Advanced;
using NCrontab.Advanced.Enumerations;

namespace EasyScheduler.Tiny
{
    public class CronTrigger : ITrigger
    {
        private CrontabSchedule _CronInstance;

        public CronTrigger(string jobName, string cronExpression)
        {
            JobName = jobName;
            _CronInstance = CrontabSchedule.Parse(cronExpression,CronStringFormat.WithSecondsAndYears);
            FirstFireTime = _CronInstance.GetNextOccurrence(DateTime.Now);
            ReadyToFire = true;
        }

        public string JobName { get; private set; }
        public bool ReadyToFire { get;  set; }
        public DateTime FirstFireTime { get; private set; }
        public DateTime? LastFireTime { get; private set; }
        public DateTime GetNextFireTime()
        {
            return _CronInstance.GetNextOccurrence(DateTime.Now);
        }

        public DateTime CurrentFireTime { get;  set; }
    }
}