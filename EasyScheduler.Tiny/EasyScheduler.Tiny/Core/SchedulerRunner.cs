using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core.Settings;

namespace EasyScheduler.Tiny.Core
{
    internal class SchedulerRunner
    {
        private readonly TaskDeliveryManager _TaskDeliveryManager;
        private readonly SchedulerSetting _SchedulerSetting;
        private FetchCycle _FetchCycle;

        public SchedulerRunner(SchedulerSetting schedulerSetting, TaskDeliveryManager taskDeliveryManager)
        {
            _SchedulerSetting = schedulerSetting;
            _TaskDeliveryManager = taskDeliveryManager;
        }

        public void Run(JobStore jobStore, TriggerStore triggerStore)
        {
            _FetchCycle = new FetchCycle(DateTime.Now,_SchedulerSetting.FetchRange);
            while (true)
            {
                Console.WriteLine("Main loop Run start at: " + DateTime.Now);
                Console.WriteLine("Main loop Run init with min: " + _FetchCycle.MinNextFireTime);
                Console.WriteLine("Main loop Run init with max: " + _FetchCycle.MaxNextFireTime);
                List<ITrigger> triggersToBeFired;
                if (!triggerStore.TryGetTriggersToBeFired(out triggersToBeFired, _FetchCycle))
                {
                    Thread.Sleep(new TimeSpan(0,0,10));
                    _FetchCycle.PushForward(_SchedulerSetting.RunnerCycleIncrement);
                    continue;
                }

                var minCurrentFireTime = triggersToBeFired.Min(x => x.CurrentFireTime);
                var timeSpan = _FetchCycle.GetMinTimeSpanToBePushForward(_SchedulerSetting.RunnerCycleIncrement, minCurrentFireTime);
                _FetchCycle.PushForward(timeSpan);
                Console.WriteLine("Main loop deliver at: " + DateTime.Now);
                var jobExecutionList = jobStore.GetJobsToBeExcuted(triggersToBeFired);
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),
                    TaskCreationOptions.LongRunning);
                //todo set trigger ready again
                if (_FetchCycle.MinNextFireTime - DateTime.Now > new TimeSpan(0, 0, 1))
                {
                    Thread.Sleep(_FetchCycle.MinNextFireTime - DateTime.Now);
                }
            }
        }
    }
}