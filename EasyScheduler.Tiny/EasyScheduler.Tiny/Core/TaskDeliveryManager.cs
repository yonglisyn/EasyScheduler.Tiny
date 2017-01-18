using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core.EnumsConstants;
using EasyScheduler.Tiny.Core.Settings;
using Newtonsoft.Json;

namespace EasyScheduler.Tiny.Core
{
    public class TaskDeliveryManager
    {
        private ConcurrentDictionary<string, Task<JobExecDetail>> _Tasks;
        private readonly TaskDeliveryManagerSetting _DeliveryManagerSetting;
        private readonly JobNotificationCenter _JobNotificationCenter;

        public TaskDeliveryManager(TaskDeliveryManagerSetting deliveryManagerSetting, JobNotificationCenter jobNotificationCenter)
        {
            _DeliveryManagerSetting = deliveryManagerSetting;
            _JobNotificationCenter = jobNotificationCenter;
            _Tasks = new ConcurrentDictionary<string, Task<JobExecDetail>>();
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
            var b = _Tasks == null ? "null in thread:" : _Tasks.Count.ToString();
            Console.WriteLine("36:::"+b +":::"+ Thread.CurrentThread.ManagedThreadId);
            await Task.WhenAll(deliverTask);
            var c = _Tasks == null ? "null in thread:" : _Tasks.Count.ToString();
            Console.WriteLine("39:::"+c + ":::" + Thread.CurrentThread.ManagedThreadId);
            while (_Tasks.Count>0)
            {
                try
                {
                    var task = await Task.WhenAny(_Tasks.Values);
                    Console.WriteLine(task);
                    var result = await task;
                    Console.WriteLine("48 "+JsonConvert.SerializeObject(result));
                    //todo test cover here
                    Task<JobExecDetail> tmp;
                    var d = _Tasks == null ? "null in thread:" : _Tasks.Count.ToString();
                    Console.WriteLine("52:::" + d + ":::" + Thread.CurrentThread.ManagedThreadId); _Tasks.TryRemove(result.JobName, out tmp);
                    JobStore.TryUpdateJobStatus(result.JobName, JobStatus.Running);
                    _JobNotificationCenter.NotifyResult(result);
                }
                catch (Exception e)
                {
                    //job exception
                    //todo handle
                    Console.WriteLine(e.Message+"\n"+e.StackTrace);

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
                    Console.WriteLine("TryDeliver RUNRURNRURR "+DateTime.Now);
                    _Tasks.TryAdd(job.JobName,Task.Run(async ()=> await job.ExcecuteAsync()));
                    Console.WriteLine("TryDeliver RUNRURNRURR 2" + DateTime.Now);
                    job.JobStatus = JobStatus.Running;
                    JobStore.TryUpdate(job);
                    _JobNotificationCenter.NotifyJobSwitchStatus(job.JobStatus, trigger.CurrentFireTime);
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
    }
}