using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core.EnumsConstants;
using EasyScheduler.Tiny.Core.Settings;

namespace EasyScheduler.Tiny.Core
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
            Console.WriteLine("Deliver start at:"+ DateTime.Now);
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
                    _JobNotificationCenter.NotifyResult(result);
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
            Console.WriteLine("TryDeliver start at:" + DateTime.Now);
            Console.WriteLine("TryDeliver trigger time:" + triggerTime);
            while (true)
            {
                var now = DateTime.Now;
                if (triggerTime >= now.AddMilliseconds(-10) && triggerTime <= now.AddMilliseconds(10))
                {
                    //Todo check what happens if not async await
                    _Tasks.Add(Task.Run(async () => await job.ExcecuteAsync()));
                    _JobNotificationCenter.NotifyJobSwitchStatus(JobExcecutionStatus.Running, trigger.CurrentFireTime);
                    break;
                }
                if (triggerTime < now)
                {
                    Console.WriteLine("TryDeliver failed with triggertime " +triggerTime + " now" + now);
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
}