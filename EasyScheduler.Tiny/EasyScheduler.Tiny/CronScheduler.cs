using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public class CronScheduler: IScheduler
    {
        private Thread _Thread;
        private JobStore _JobStore;
        private TiggerStore _TiggerStore;
        private SchedulerStatus _SchedulerStatus;
        private TaskDeliveryManager _TaskDeliveryManager;

        public CronScheduler()
        {
            _JobStore = new JobStore();
            _TiggerStore = new TiggerStore();
            _TaskDeliveryManager = new TaskDeliveryManager();
        }

        public IJob GetJob(string jobName)
        {
            return _JobStore.Get(jobName);
        }

        public ITrigger GetTrigger(string jobName)
        {
            return _TiggerStore.GetTriggerBy(jobName);
        }

        public void Schedule(IJob job, ITrigger trigger)
        {
            _JobStore.Add(job);
            _TiggerStore.Add(trigger);
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
        }

        private void Run()
        {
            _SchedulerStatus = SchedulerStatus.Running;
            var maxNextFireTime = DateTime.Now;
            while (_SchedulerStatus == SchedulerStatus.Running)
            {
                var timeSpan = new TimeSpan(0, 1, 0);
                maxNextFireTime = maxNextFireTime + timeSpan;
                var triggersToBeFired = _TiggerStore.GetTriggersToBeFired(maxNextFireTime);
                var jobExecutionList = _JobStore.GetJobsToBeExcuted(triggersToBeFired);
                Task.Factory.StartNew(() => _TaskDeliveryManager.Deliver(jobExecutionList, triggersToBeFired),
                    TaskCreationOptions.LongRunning);
                    //.Run(()=>);
                Thread.Sleep(timeSpan);
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

    internal enum SchedulerStatus
    {
        Started = 1,
        Running = 2,
        Stopped = 3,
        Paused = 4
    }
}
