using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core;
using EasyScheduler.Tiny.Core.Enums;
using EasyScheduler.Tiny.Core.Settings;
using Moq;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture]
    public class CronSchedulerTest
    {
        private readonly SchedulerSetting _SchedulerSetting = SchedulerSetting.Default();
        private readonly TaskDeliveryManagerSetting _TaskDeliveryManagerSetting = TaskDeliveryManagerSetting.Default();
        [Test]
        public void ScheduleJob_ShouldAddJobToJobStore_And_ShouldAddTriggerToTriggerStore()
        {
            IJob job = new SimpleJob(typeof(SimpleJob).Name,typeof(SimpleTrigger).ToString());
            ITrigger trigger = new SimpleTrigger(typeof(SimpleJob).Name);

            var target = new CronScheduler(_SchedulerSetting,new TaskDeliveryManager(_TaskDeliveryManagerSetting,new JobNotificationCenter()));
            target.Schedule(job,trigger);

            var resultJob = target.GetJob(typeof (SimpleJob).Name);
            Assert.AreEqual(resultJob.JobName, job.JobName);

            var resultTrigger = target.GetTrigger(typeof (SimpleJob).Name);
            Assert.AreEqual(resultTrigger.JobName,trigger.JobName);
        }

        [Test]
        public void ExecuteOneJob_BasedOnTrigger()
        {
            var jobNormalMoq = new Mock<IJob>();
            jobNormalMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobNormalMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            IJob jobNormal = jobNormalMoq.Object;
            string cronExpression = "0/1 * * * * * *";
            ITrigger tigger = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
                target.Start();
                target.Schedule(jobNormal, tigger);
            Thread.Sleep(new TimeSpan(0,0,12));
            jobNormalMoq.Verify(x => x.ExcecuteAsync(), Times.AtLeastOnce);
        }

        [Test]
        public void ExecuteOneJob_MoreThanTwice_BasedOnTrigger()
        {
            var jobNormalMoq = new Mock<IJob>();
            jobNormalMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobNormalMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            IJob jobNormal = jobNormalMoq.Object;
            string cronExpression = "0/1 * * * * * *";
            ITrigger tigger = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
                target.Start();
                target.Schedule(jobNormal, tigger);
            Thread.Sleep(new TimeSpan(0,0,14));
            jobNormalMoq.Verify(x => x.ExcecuteAsync(), Times.AtLeast(2));
        }

        [Test]
        public void Execute2Jobs_BasedOnTriggers()
        {
            List<IJob> jobs = new List<IJob>();
            List<ITrigger> triggers;
            var jobMoq = new Mock<IJob>();
            jobMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            jobs.Add(jobMoq.Object);
            var jobMoq2 = new Mock<IJob>();
            jobMoq2.SetupGet(x => x.JobName).Returns("SimpleJob2");
            jobMoq2.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            jobs.Add(jobMoq2.Object);
            string cronExpression = "0/1 * * * * * *";
            var triggerstmp = jobs.Select(x => new CronTrigger(x.JobName, cronExpression)).ToList();
            triggers = new List<ITrigger>(triggerstmp);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
                target.Start();
                jobs.ForEach(x=>target.Schedule(x,triggers.First(y=>y.JobName==x.JobName)));
            Thread.Sleep(new TimeSpan(0,0,12));
            jobMoq.Verify(x => x.ExcecuteAsync(), Times.AtLeastOnce);
            jobMoq2.Verify(x => x.ExcecuteAsync(), Times.AtLeastOnce);
        }

        [Test]
        public void OneJobThrowException_ShouldNotAffectOtherJobs()
        {
            var jobMoq = new Mock<IJob>();
            jobMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            var simpleJobThrowException = new SimpleJobThrowException("SimpleJobThrowException");
            string cronExpression = "0/2 * * * * * *";
            string cronExpression2 = "0/1 * * * * * *";
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            target.Schedule(simpleJobThrowException, new CronTrigger("SimpleJobThrowException",cronExpression2));
            target.Schedule(jobMoq.Object, new CronTrigger("SimpleJob", cronExpression));
            Thread.Sleep(new TimeSpan(0, 0, 13));
            jobMoq.Verify(x => x.ExcecuteAsync(), Times.AtLeastOnce);
        }


        [Test]
        public void Scheduler_ShouldStop_IfCancellationRequested()
        {
            //var jobNormalMoq = new Mock<IJob>();
            //jobNormalMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            //jobNormalMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExcecutionResult>.Factory.StartNew(() => JobExcecutionResult.Success));
            //IJob jobNormal = jobNormalMoq.Object;
            //string cronExpression = "0/1 * * * * * *";
            //ITrigger tigger = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            //target.Schedule(jobNormal, tigger);
            Thread.Sleep(new TimeSpan(0, 0, 2));
            target.Stop();
            Assert.AreEqual(SchedulerStatus.Stopped,target.SchedulerStatus);

        }
    }

    public class SimpleJobThrowException : IJob
    {
        public SimpleJobThrowException(string toString)
        {
            JobName = toString;
        }

        public string JobName { get; private set; }

        public async Task<JobExcecutionResult> ExcecuteAsync()
        {
            await Task.Delay(10);
            Console.WriteLine("I am fired at " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return JobExcecutionResult.Success;
        }

    }

    public class SimpleTrigger : ITrigger
    {
        private string _JobName;
        public SimpleTrigger(string jobName)
        {
            _JobName = jobName;
        }

        public string JobName { get { return _JobName; } }
        public DateTime FirstFireTime { get; private set; }
        public DateTime? LastFireTime { get; private set; }
        public DateTime GetNextFireTime(DateTime baseValue)
        {
            throw new NotImplementedException();
        }

        public DateTime CurrentFireTime { get; set; }
    }

    public class SimpleJob : IJob
    {
        private string _TiggerName;

        public SimpleJob(string jobName, string tiggerName)
        {
            JobName = jobName;
            _TiggerName = tiggerName;
        }

        public string JobName { get; private set; }

        public async Task<JobExcecutionResult> ExcecuteAsync()
        {
            await Task.Delay(10);
            Console.WriteLine("I am fired at "+DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return JobExcecutionResult.Success;
        }

    }
}
