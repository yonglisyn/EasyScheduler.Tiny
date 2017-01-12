using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny.Core
{
    internal class TriggerStore
    {
        private ConcurrentDictionary<string,ITrigger> _Triggers;

        public TriggerStore()
        {
            _Triggers = new ConcurrentDictionary<string, ITrigger>();
        }

        public ITrigger GetTriggerBy(string jobName)
        {
            ITrigger trigger;
            _Triggers.TryGetValue(jobName, out trigger);
            return trigger;
        }

        public bool TryAdd(ITrigger trigger)
        {
            return _Triggers.TryAdd(trigger.JobName, trigger);
        }


        public bool TryGetTriggersToBeFired(out List<ITrigger> toBeFired, FetchCycle fetchCycle)
        {
            if (_Triggers.Count == 0)
            {
                toBeFired= new List<ITrigger>();
                return false;
            }
            toBeFired =
                _Triggers.Values.Where(x => 
                    x.GetNextFireTime(fetchCycle.MinNextFireTime) <= fetchCycle.MaxNextFireTime 
                    && (x.CurrentFireTime == DateTime.MinValue || x.CurrentFireTime < fetchCycle.MinNextFireTime))
                    .ToList();
            if (toBeFired.Count == 0)
            {
                var tmp = _Triggers.Values.First().GetNextFireTime(fetchCycle.MinNextFireTime);
                Console.WriteLine("TryGetTriggersToBeFired failed for min: " + tmp);
                return false;
            }
            toBeFired.ForEach(x => UpdateTrigger(x, fetchCycle.MinNextFireTime));
            return true;   
        }

        private void UpdateTrigger(ITrigger trigger, DateTime baseValue)
        {
            //todo review may not need this CurrentFireTime
            trigger.CurrentFireTime = trigger.GetNextFireTime(baseValue);
        }
    }
}