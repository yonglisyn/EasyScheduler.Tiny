using System;
using System.Threading;
using EasyScheduler.Tiny.Core.Enums;
using EasyScheduler.Tiny.Core.Exceptions;
using EasyScheduler.Tiny.Core.Settings;

namespace EasyScheduler.Tiny.Core
{
    public class CronScheduler: IScheduler
    {
        private Thread _Thread;
        private JobStore _JobStore;
        private TriggerStore _TriggerStore;
        private SchedulerStatus _SchedulerStatus;
        private readonly SchedulerRunner _SchedulerRunner;

        public CronScheduler(SchedulerSetting schedulerSetting, TaskDeliveryManager taskDeliveryManager)
        {
            _SchedulerRunner = new SchedulerRunner(schedulerSetting,taskDeliveryManager);
            _JobStore = new JobStore();
            _TriggerStore = new TriggerStore();
        }

        public CronScheduler()
        {
            var schedulerSetting = SchedulerSetting.Default();
            _SchedulerRunner = new SchedulerRunner(schedulerSetting, new TaskDeliveryManager(TaskDeliveryManagerSetting.Default(), new JobNotificationCenter()));
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
            Console.WriteLine("Scheduled on " + DateTime.Now);
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
            try
            {
                _Thread.Start();
            }
            catch (Exception ex)
            {
                Stop();
                //todo logprovider
                Console.WriteLine(ex.Message + "\n"+ex.StackTrace);
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
}