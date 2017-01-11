using System;
using NCrontab.Advanced;
using NCrontab.Advanced.Enumerations;

namespace EasyScheduler.Tiny
{
//    Field name   | Allowed values  | Allowed special characters
//------------------------------------------------------------
//Minutes      | 0-59            | * , - /
//Hours        | 0-23            | * , - /
//Day of month | 1-31            | * , - / ? L W
//Month        | 1-12 or JAN-DEC | * , - /
//Day of week  | 0-6 or SUN-SAT  | * , - / ? L #
//Year         | 0001–9999       | * , - /
    public class CronTrigger : ITrigger
    {
        private CrontabSchedule _CronInstance;

        public CronTrigger(string jobName, string cronExpression)
        {
            JobName = jobName;
            _CronInstance = CrontabSchedule.Parse(cronExpression,CronStringFormat.WithSecondsAndYears);
            FirstFireTime = _CronInstance.GetNextOccurrence(DateTime.Now);
        }

        public string JobName { get; private set; }
        public DateTime FirstFireTime { get; private set; }
        public DateTime? LastFireTime { get; private set; }
        public DateTime GetNextFireTime(DateTime baseValue)
        {
            return _CronInstance.GetNextOccurrence(baseValue);
        }

        public DateTime CurrentFireTime { get;  set; }
    }
}