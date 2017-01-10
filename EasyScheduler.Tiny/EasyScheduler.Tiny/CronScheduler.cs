using System;
using System.Threading;

namespace EasyScheduler.Tiny
{
    public class CronScheduler: IScheduler
    {
        private Thread _Thread;
        private JobStore _JobStore;
        private TriggerStore _TriggerStore;
        private SchedulerStatus _SchedulerStatus;
        private readonly SchedulerSetting _SchedulerSetting;
        private object _StoreLock = new object();
        private readonly SchedulerRunner _SchedulerRunner;

        public CronScheduler(SchedulerSetting schedulerSetting, TaskDeliveryManager taskDeliveryManager)
        {
            _SchedulerSetting = schedulerSetting;
            _SchedulerRunner = new SchedulerRunner(_SchedulerSetting,taskDeliveryManager);
            _JobStore = new JobStore();
            _TriggerStore = new TriggerStore();
        }

        public CronScheduler()
        {
            _SchedulerSetting = SchedulerSetting.Default();
            _SchedulerRunner = new SchedulerRunner(_SchedulerSetting, new TaskDeliveryManager(TaskDeliveryManagerSetting.Default(), new JobNotificationCenter()));
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
            lock (_StoreLock)
            {
                _JobStore.Add(job);
                _TriggerStore.TryAdd(trigger);
            }
            Console.WriteLine("Scheduled on "+DateTime.Now);
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
            _Thread = new Thread(()=>_SchedulerRunner.Run(_JobStore,_TriggerStore));
            _SchedulerStatus = SchedulerStatus.Running;
            _Thread.Start();
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
