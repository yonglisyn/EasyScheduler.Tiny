using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny
{
    internal class TriggerStore
    {
        private static ConcurrentDictionary<string,ITrigger> _Triggers;

        static TriggerStore()
        {
            _Triggers = new ConcurrentDictionary<string, ITrigger>();
        }

        public ITrigger GetTriggerBy(string jobName)
        {
            return _Triggers.FirstOrDefault(x => x.Key == jobName).Value;
        }

        public bool TryAdd(ITrigger trigger)
        {
            return _Triggers.TryAdd(trigger.JobName, trigger);
        }

        public bool TryGetTriggersToBeFired(DateTime minNextFireTime, DateTime maxNextFireTime, out List<ITrigger> toBeFired, DateTime baseValue)
        {
            toBeFired =
                _Triggers.Values.Where(x => x.GetNextFireTime(baseValue)>=minNextFireTime && x.GetNextFireTime(baseValue)<= maxNextFireTime && x.ReadyToFire)
                    .ToList();
            if (toBeFired.Count == 0)
            {
                return false;
            }
            toBeFired.ForEach(x=>UpdateTrigger(x,baseValue));
            return true;   
        }

        private void UpdateTrigger(ITrigger trigger, DateTime baseValue)
        {
            trigger.ReadyToFire = false;
            trigger.CurrentFireTime = trigger.GetNextFireTime(baseValue);
        }
    }
}