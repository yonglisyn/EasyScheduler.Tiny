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
            while (jobExecutionList.Count>0)
            {
                foreach (var trigger in triggersToBeFired)
                {
                    var job = jobExecutionList.First(y => y.JobName == trigger.JobName);
                    if (TryDeliver(trigger, job))
                    {
                        _JobNotificationCenter.BroadCast(JobExcecutionStatus.Running, trigger.CurrentFireTime);
                        jobExecutionList.Remove(job);
                        triggersToBeFired.Remove(trigger);
                    }
                }
                Thread.Sleep(_DeliveryManagerSetting.TaskDeliveryIdleCycle);
            }
            while (_Tasks.Count>0)
            {
                var task = await Task.WhenAny(_Tasks);
                _Tasks.Remove(task);
                NotifyJobListeners(await task);
            }
        }

        private bool TryDeliver(ITrigger trigger, IJob job)
        {
            var now = DateTime.Now;
            var triggerTime = trigger.CurrentFireTime;
            if (triggerTime.AddMilliseconds(10) >= now && triggerTime.AddMilliseconds(-10) <= now)
            {
                _Tasks.Add(Task.Run(()=>job.Excecute()));
                return true;
            }
            return false;
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