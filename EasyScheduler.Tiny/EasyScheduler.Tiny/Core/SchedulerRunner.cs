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

        public void Run(CancellationToken token)
        {
            _FetchCycle = new FetchCycle(DateTime.Now,_SchedulerSetting.FetchRange);
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (token.IsCancellationRequested)
                {
                    //todo replace with logprovider to log where
                    Console.WriteLine("cancel requested");
                    throw new OperationCanceledException();
                }
                Console.WriteLine("Main loop Run init with min: " + _FetchCycle.MinNextFireTime);
                List<ITrigger> triggersToBeFired;
                if (!TriggerStore.TryGetTriggersToBeFired(out triggersToBeFired, _FetchCycle))
                {
                    Thread.Sleep(new TimeSpan(0,0,10));
                    _FetchCycle.PushForward(_SchedulerSetting.RunnerCycleIncrement);
                    continue;
                }

                var minCurrentFireTime = triggersToBeFired.Min(x => x.CurrentFireTime);
                var timeSpan = _FetchCycle.GetMinTimeSpanToBePushForward(_SchedulerSetting.RunnerCycleIncrement, minCurrentFireTime);
                _FetchCycle.PushForward(timeSpan);
                var jobExecutionList = JobStore.GetJobsToBeExcuted(triggersToBeFired.Select(x=>x.JobName).ToList());
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
                //todo set trigger ready again
                if (_FetchCycle.MinNextFireTime - DateTime.Now > new TimeSpan(0, 0, 1))
                {
                    Thread.Sleep(_FetchCycle.MinNextFireTime - DateTime.Now);
                }
            }
        }
    }
}