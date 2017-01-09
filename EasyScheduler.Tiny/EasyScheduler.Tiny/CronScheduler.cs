﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public class CronScheduler: IScheduler
    {
        private Thread _Thread;
        private JobStore _JobStore;
        private TriggerStore _TriggerStore;
        private SchedulerStatus _SchedulerStatus;
        private readonly TaskDeliveryManager _TaskDeliveryManager;
        private readonly SchedulerSetting _SchedulerSetting;

        public CronScheduler(SchedulerSetting schedulerSetting, TaskDeliveryManager taskDeliveryManager)
        {
            _SchedulerSetting = schedulerSetting;
            _TaskDeliveryManager = taskDeliveryManager;
            _JobStore = new JobStore();
            _TriggerStore = new TriggerStore();
        }

        public CronScheduler()
        {
            _SchedulerSetting = SchedulerSetting.Default();
            _TaskDeliveryManager = new TaskDeliveryManager(TaskDeliveryManagerSetting.Default(), new JobNotificationCenter());
            _JobStore = new JobStore();
            _TriggerStore = new TriggerStore();
        }

        public IJob GetJob(string jobName)
        {
            return _JobStore.Get(jobName);
        }

        public ITrigger GetTrigger(string jobName)
        {
            return _TriggerStore.GetTriggerBy(jobName);
        }

        public void Schedule(IJob job, ITrigger trigger)
        {
            if (!string.Equals(job.JobName,trigger.JobName,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new EasySchedulerException(string.Format("IJob {0} and ITrigger {1} must have same JobName!", job.JobName, trigger.JobName));
            }
            _JobStore.Add(job);
            _TriggerStore.TryAdd(trigger);
        }

        public void Disable(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Enable(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            _SchedulerStatus = SchedulerStatus.Started;
            //TODO NotifySchedulerListeners 
            Console.WriteLine("Start 74 " + DateTime.Now);

            _Thread = new Thread(Run);
            Console.WriteLine("Start 77 " + DateTime.Now);

            _Thread.Start();
        }

        private void Run()
        {
            _SchedulerStatus = SchedulerStatus.Running;
            var minNextFireTime = DateTime.Now;
            var maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange;
            while (_SchedulerStatus == SchedulerStatus.Running)
            {
                Console.WriteLine("Run 89 " + DateTime.Now);
                Console.WriteLine("Run min " + minNextFireTime + " Rum max: " + maxNextFireTime);
                List<ITrigger> triggersToBeFired;
                if (!_TriggerStore.TryGetTriggersToBeFired(minNextFireTime, maxNextFireTime, out triggersToBeFired))
                {
                    Thread.Sleep(new TimeSpan(0,0,10));
                    minNextFireTime = DateTime.Now;
                    maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange; 
                    continue;
                }
                Console.WriteLine("Run 98 "+DateTime.Now);
                minNextFireTime = triggersToBeFired.Min(x=>x.CurrentFireTime);
                maxNextFireTime = minNextFireTime + _SchedulerSetting.FetchTriggersRange;
                Console.WriteLine("Run 100 minNextFireTime " + minNextFireTime);
                Console.WriteLine("Run 101 maxNextFireTime" + maxNextFireTime);
                var jobExecutionList = _JobStore.GetJobsToBeExcuted(triggersToBeFired);
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),
                    TaskCreationOptions.LongRunning);
                //todo set trigger ready again
                Thread.Sleep(minNextFireTime-DateTime.Now);
               
            }
        }

        public void Stop()
        {
            _SchedulerStatus = SchedulerStatus.Stopped;
            _Thread.Join();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SchedulerSetting
    {
        private readonly TimeSpan _FetchTriggersRange;
        private readonly TimeSpan _SchedulerIdleTime;

        public TimeSpan FetchTriggersRange { get { return _FetchTriggersRange; } }
        public TimeSpan SchedulerIdleTime { get { return _SchedulerIdleTime; } }
        public SchedulerSetting(TimeSpan fetchTriggersRange, TimeSpan schedulerIdleTime)
        {
            _FetchTriggersRange = fetchTriggersRange;
            _SchedulerIdleTime = schedulerIdleTime;
        }

        public static SchedulerSetting Default()
        {
            return new SchedulerSetting(new TimeSpan(0,5,0),new TimeSpan(0,0,10));
        }
    }

    internal enum SchedulerStatus
    {
        Started = 1,
        Running = 2,
        Stopped = 3,
        Paused = 4
    }
}
