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
            //todo Task.Run error handle
            Console.WriteLine("Deliver 24:"+ DateTime.Now);
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
            Console.WriteLine("TryDeliver 51:"+DateTime.Now);

            var triggerTime = trigger.CurrentFireTime;
            Console.WriteLine("TryDeliver 54:"+triggerTime);
            while (true)
            {
                var now = DateTime.Now;
                if (triggerTime >= now.AddMilliseconds(-10) && triggerTime <= now.AddMilliseconds(10))
                {
                    //Todo check what happens if not async await
                    _Tasks.Add(Task.Run(async () => await job.ExcecuteAsync()));
                    _JobNotificationCenter.BroadCast(JobExcecutionStatus.Running, trigger.CurrentFireTime);
                    break;
                }
                if (triggerTime < now)
                {
                    Console.WriteLine("TryDeliver special triggertime " +triggerTime + " now" + now);
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