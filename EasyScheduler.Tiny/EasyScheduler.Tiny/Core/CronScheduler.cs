using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
        private Task _MainTask;

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
        public SchedulerStatus SchedulerStatus { get { return _SchedulerStatus; } }

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
            if (!string.Equals(job.JobName, trigger.JobName, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new EasySchedulerException(string.Format("IJob {0} and ITrigger {1} must have same JobName!",
                    job.JobName, trigger.JobName));
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
            //try
            //{
                _MainTask = Task.Factory.StartNew(() => _SchedulerRunner.Run(_JobStore, _TriggerStore, _CancellationTokenSource.Token),
                    _CancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default)
                    .ContinueWith(task => SchedulerNotificationCenter.NotifyStatus(SchedulerStatus.Stopped), TaskContinuationOptions.OnlyOnCanceled);

                //Task.Run(() => _SchedulerRunner.Run(_JobStore, _TriggerStore, _CancellationTokenSource.Token),
                //    _CancellationTokenSource.Token,
                //    TaskCreationOptions.LongRunning, TaskScheduler.Default)
                //    .ContinueWith(task => SchedulerNotificationCenter.NotifyStatus(SchedulerStatus.Stopped), TaskContinuationOptions.OnlyOnCanceled);
            //}
            //catch (OperationCanceledException ex)
            //{
            //    Console.WriteLine("I am exception");
            //}
            Console.WriteLine("I am over");
            _SchedulerStatus = SchedulerStatus.Running;
        }

        public void Stop()
        {
            _CancellationTokenSource.Cancel();
            Console.WriteLine("Cancel requested");
            _MainTask.Wait();
            _SchedulerStatus = SchedulerStatus.Stopped;
            //Todo notify schedulerlistener stoped
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SchedulerNotificationCenter
    {
        public static void NotifyStatus(SchedulerStatus stopped)
        {
            Console.WriteLine("Cancelled");
        }
    }
}