using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public class SchedulerRunner
    {
        private readonly TaskDeliveryManager _TaskDeliveryManager;
        private SchedulerSetting _SchedulerSetting;

        public SchedulerRunner(SchedulerSetting schedulerSetting, TaskDeliveryManager taskDeliveryManager)
        {
            _SchedulerSetting = schedulerSetting;
            _TaskDeliveryManager = taskDeliveryManager;
        }
        public void Run(JobStore jobStore, TriggerStore triggerStore)
        {
            var minNextFireTime = DateTime.Now;
            var maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange;
            while (true)
            {
                Console.WriteLine("Main loop Run start at: " + DateTime.Now);
                Console.WriteLine("Main loop Run init with min: " + minNextFireTime);
                Console.WriteLine("Main loop Run init with max: " + maxNextFireTime);
                List<ITrigger> triggersToBeFired;
                if (!triggerStore.TryGetTriggersToBeFired(minNextFireTime, maxNextFireTime, out triggersToBeFired))
                {
                    Thread.Sleep(new TimeSpan(0,0,10));
                    minNextFireTime = minNextFireTime + _SchedulerSetting.RunnerCycleIncrement;
                    maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange; 
                    continue;
                }
                minNextFireTime = minNextFireTime + new TimeSpan(0, 0, 10);
                if (minNextFireTime > triggersToBeFired.Min(x => x.CurrentFireTime))
                {
                    minNextFireTime = triggersToBeFired.Min(x => x.CurrentFireTime);
                }
                maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange;
                Console.WriteLine("Main loop deliver at: " + DateTime.Now);
                var jobExecutionList = jobStore.GetJobsToBeExcuted(triggersToBeFired);
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),
                    TaskCreationOptions.LongRunning);
                //todo set trigger ready again
                Thread.Sleep(minNextFireTime-DateTime.Now);
               
            }
        }
    }
}