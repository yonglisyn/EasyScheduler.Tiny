using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public class TaskDeliveryManager
    {
        private List<Task<JobExcecutionResult>> _Tasks;
        private readonly TaskDeliveryManagerSetting _DeliveryManagerSetting;
        private readonly JobNotificationCenter _JobNotificationCenter;

        public TaskDeliveryManager(TaskDeliveryManagerSetting deliveryManagerSetting, JobNotificationCenter jobNotificationCenter)
        {
            _DeliveryManagerSetting = deliveryManagerSetting;
            _JobNotificationCenter = jobNotificationCenter;
        }

        public async void Deliver(List<IJob> jobExecutionList, List<ITrigger> triggersToBeFired)
        {
            var deliverTask = new List<Task>();
            foreach (var trigger in triggersToBeFired)
            {
                var job = jobExecutionList.First(y => y.JobName == trigger.JobName);
                deliverTask.Add(Task.Run(() => TryDeliver(trigger, job)));
            }
            await Task.WhenAll(deliverTask);
            while (_Tasks.Count>0)
            {
                try
                {
                    var task = await Task.WhenAny(_Tasks);
                    _Tasks.Remove(task);
                    var result = await task;
                    NotifyJobListeners(result);
                }
                catch (Exception)
                {
                   //job exception
                    //todo handle
                }
            }
        }

        private void TryDeliver(ITrigger trigger, IJob job)
        {
            var triggerTime = trigger.CurrentFireTime;
            while(true)
            {
                var now = DateTime.Now;
                if (triggerTime >= now.AddMilliseconds(-10) && triggerTime <= now.AddMilliseconds(10))
                {
                    _Tasks.Add(Task.Run(() => job.Excecute()));
                    _JobNotificationCenter.BroadCast(JobExcecutionStatus.Running, trigger.CurrentFireTime);
                    break;
                }
                if (triggerTime < now)
                {
                    //todo log job push to start failed
                    break;
                }
                Thread.Sleep(10);
            }
        }

        private void NotifyJobListeners(JobExcecutionResult result)
        {
            
        }
    }

    public enum JobExcecutionStatus
    {
        Idle = 1,
        Running = 2,
        FinishedSuccessful = 3,
        Failed = 4
    }

    public class JobNotificationCenter
    {
        public void BroadCast(JobExcecutionStatus started, DateTime currentFireTime)
        {
            //todo need to be fire and forget to do not take up time to continue next loop
        }
    }

    public class TaskDeliveryManagerSetting
    {
        private readonly TimeSpan _TaskDeliveryIdleCycle;

        private TaskDeliveryManagerSetting(TimeSpan taskDeliveryIdleCycle)
        {
            _TaskDeliveryIdleCycle = taskDeliveryIdleCycle;
        }

        public TimeSpan TaskDeliveryIdleCycle { get { return _TaskDeliveryIdleCycle; } }

        public static TaskDeliveryManagerSetting Default()
        {
            return new TaskDeliveryManagerSetting(new TimeSpan(0,0,0,0,10));
        }
    }
}