using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny
{
    internal class TiggerStore
    {
        private ConcurrentBag<ITrigger> _Triggers;

        internal TiggerStore()
        {
            _Triggers = new ConcurrentBag<ITrigger>();
        }

        public ITrigger GetTriggerBy(string jobName)
        {
            return _Triggers.FirstOrDefault(x => x.JobName == jobName);
        }

        public void Add(ITrigger trigger)
        {
            _Triggers.Add(trigger);
        }

        public List<ITrigger> GetTriggersToBeFired(DateTime maxNextFireTime)
        {
            var triggersToBeFired =
                _Triggers.Where(x => DateTime.Compare(x.GetNextFireTime(), maxNextFireTime) < 0 && x.ReadyToFire)
                    .ToList();
            triggersToBeFired.ForEach(trigger=>trigger.ReadyToFire = false);
            return triggersToBeFired;   
        }

    }
}