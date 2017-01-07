using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _Thread = new Thread(Run);
            _Thread.Start();
            _Thread.Join();
        }

        private void Run()
        {
            _SchedulerStatus = SchedulerStatus.Running;
            var maxNextFireTime = DateTime.Now+_SchedulerSetting.FetchTriggersRange;
            var minNextFireTime = DateTime.Now;
            while (_SchedulerStatus == SchedulerStatus.Running)
            {
                List<ITrigger> triggersToBeFired;
                if (!_TriggerStore.TryGetTriggersToBeFired(minNextFireTime, maxNextFireTime, out triggersToBeFired, DateTime.Now))
                {
                    minNextFireTime = maxNextFireTime;
                    maxNextFireTime = maxNextFireTime + _SchedulerSetting.FetchTriggersRange;
                    continue;
                }
                minNextFireTime = maxNextFireTime;
                maxNextFireTime = maxNextFireTime + _SchedulerSetting.FetchTriggersRange;
                //record time span between this fetch and next fetch; stop scheduler if this time span larger than FetchTriggersRange
                var timeChecker = new Stopwatch();
                timeChecker.Start();
                var jobExecutionList = _JobStore.GetJobsToBeExcuted(triggersToBeFired);
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),
                    TaskCreationOptions.LongRunning);
                Thread.Sleep(_SchedulerSetting.SchedulerIdleTime);
                timeChecker.Stop();
                if(timeChecker.Elapsed > _SchedulerSetting.FetchTriggersRange)
                { _SchedulerStatus = SchedulerStatus.Stopped;}
            }
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
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
