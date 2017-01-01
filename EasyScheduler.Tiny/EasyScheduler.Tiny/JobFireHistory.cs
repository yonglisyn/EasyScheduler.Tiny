using System;
using System.Collections.Generic;

namespace EasyScheduler.Tiny
{
    public class JobFireHistory
    {
        private readonly string _JobName;
        private readonly Dictionary<DateTime, TriggerStatus> _TiggerRecords;

        public string JobName { get { return _JobName; } }
        public Dictionary<DateTime, TriggerStatus> TriggerRecords { get { return _TiggerRecords; } }

        public JobFireHistory(string jobName, Dictionary<DateTime, TriggerStatus> tiggerRecords)
        {
            _JobName = jobName;
            _TiggerRecords = tiggerRecords;
        }
    }
}