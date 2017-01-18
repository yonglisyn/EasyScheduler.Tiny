using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core;
using EasyScheduler.Tiny.Core.Enums;
using EasyScheduler.Tiny.Core.EnumsConstants;
using EasyScheduler.Tiny.Core.Settings;
using Moq;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture]
    public class CronSchedulerTest:TestBase
    {
        [TearDown]
        public void TearDown()
        {
            TriggerStore.Reset();
            JobStore.Reset();
        }

        private readonly SchedulerSetting _SchedulerSetting = SchedulerSetting.Default();
        private readonly TaskDeliveryManagerSetting _TaskDeliveryManagerSetting = TaskDeliveryManagerSetting.Default();
        [Test]
        public void ScheduleJob_ShouldAddJobToJobStore_And_ShouldAddTriggerToTriggerStore()
        {
            IJob job = new SimpleJob(typeof(SimpleJob).Name, 10);
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
            jobNormalMoq.Setup(x => x.ExcecuteAsync()).Returns(async () =>
            {
                await Task.Delay(10000);
                Console.WriteLine("I am fireeeeeeeeeeeeeeeed at::"+ DateTime.Now);
                return new JobExecDetail(JobExecResult.Success, "SimpleJob");
            });
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
            jobNormalMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExecDetail>.Factory.StartNew(() => new JobExecDetail(JobExecResult.Success, "SimpleJob")));
            IJob jobNormal = jobNormalMoq.Object;
            string cronExpression = "0/1 * * * * * *";
            ITrigger trigger = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
                target.Start();
                target.Schedule(jobNormal, trigger);
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
            jobMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExecDetail>.Factory.StartNew(() => new JobExecDetail(JobExecResult.Success, "SimpleJob")));
            jobs.Add(jobMoq.Object);
            var jobMoq2 = new Mock<IJob>();
            jobMoq2.SetupGet(x => x.JobName).Returns("SimpleJob2");
            jobMoq2.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExecDetail>.Factory.StartNew(() => new JobExecDetail(JobExecResult.Success, "SimpleJob2")));
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
            jobMoq.Setup(x => x.ExcecuteAsync()).Returns(() => Task<JobExecDetail>.Factory.StartNew(() => new JobExecDetail(JobExecResult.Success, "SimpleJob")));
            var simpleJobThrowException = new SimpleJobThrowException("SimpleJobThrowException",10);
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
        public void SchedulerShouldStop_IfSchedulerStoped()
        {
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            Thread.Sleep(new TimeSpan(0, 0, 2));
            target.Stop();
            Assert.AreEqual(SchedulerStatus.Stopped,target.SchedulerStatus);

        }

        [Test]
        public void RunningJobs_ShouldContinueCurrentFireRound_IfSchedulerStoped()
        {
            
            string everySec = "0/1 * * * * * *";
            var now = DateTime.Now.Minute+2;
            string every30Sec = "0/30 "+now+ " * * * * *";
            string jobOneName = "jobOneName";
            string jobTwoName = "jobTwoName";
            ITrigger everySectrigger = new CronTrigger(jobOneName, everySec);
            ITrigger every30Sectrigger = new CronTrigger(jobTwoName, every30Sec);

            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));

            var job1 = SimpleJob(jobOneName,30000);
            target.Schedule(job1, everySectrigger);
            var job2 = SimpleJob(jobTwoName, 5000);
            target.Schedule(job2, every30Sectrigger);
            target.Start();

            Thread.Sleep(new TimeSpan(0, 0, 16));
            target.Stop();
            var jobOneActual = target.GetJob(jobOneName);
            var jobTwoActual = target.GetJob(jobTwoName);
            Assert.AreEqual(JobStatus.Running, jobOneActual.JobStatus);
            Assert.AreEqual(JobStatus.Idle, jobTwoActual.JobStatus);
            Assert.AreEqual(SchedulerStatus.Stopped, target.SchedulerStatus);
        }

        private static Mock<IJob> JobMoq(string simplejob)
        {
            var jobNormalMoq = new Mock<IJob>(){CallBase = true};
            jobNormalMoq.SetupGet(x => x.JobName).Returns(simplejob);
            jobNormalMoq.Setup(x => x.ExcecuteAsync())
                .Returns(() => Task<JobExecDetail>.Factory.StartNew(() => new JobExecDetail(JobExecResult.Success, simplejob)));
            return jobNormalMoq;
        }

        private static Mock<IJob> JobMoqWithDealy(string simplejob, TimeSpan delay)
        {
            var jobNormalMoq = new Mock<IJob> {CallBase = true};
            jobNormalMoq.SetupGet(x => x.JobName).Returns(simplejob);
            jobNormalMoq.Setup(x => x.ExcecuteAsync())
                .Returns(async () =>
                {
                    await Task.Delay(delay);
                    return new JobExecDetail(JobExecResult.Success, simplejob);
                });
            return jobNormalMoq;
        }
    }

    public class TestBase
    {
        public IJob SimpleJob(string jobName, int millisecondsDelay)
        {
            return TestJobBuilder.Build(jobName, millisecondsDelay); 
        }

        public IJob SimpleJobWithException(string jobName, int millisecondsDelay)
        {
            return TestJobBuilder.BuildException(jobName, millisecondsDelay); 
        }
    }

    internal class TestJobBuilder
    {
        public static IJob Build(string jobName, int millisecondsDelay)
        {
            return new SimpleJob(jobName, millisecondsDelay);
        }

        public static IJob BuildException(string jobName, int millisecondsDelay)
        {
            return new SimpleJobThrowException(jobName,millisecondsDelay);
        }
    }

    public class SimpleJobThrowException : IJob
    {
        private int _MillisecondsDelay;

        public SimpleJobThrowException(string jobName, int millisecondsDelay, JobStatus status = JobStatus.Idle)
        {
            JobName = jobName;
            JobStatus = status;
            _MillisecondsDelay = millisecondsDelay;
        }

        public string JobName { get; private set; }
        public JobStatus JobStatus { get; set; }

        public async Task<JobExecDetail> ExcecuteAsync()
        {
            await Task.Delay(_MillisecondsDelay);
            Console.WriteLine("I am fired at " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            throw new Exception("yo yo yo qie ke nao");
            return new JobExecDetail(JobExecResult.Success, JobName);
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
        private int _MillisecondsDelay;

        public SimpleJob(string jobName, int millisecondsDelay, JobStatus status = JobStatus.Idle)
        {
            JobName = jobName;
            JobStatus = status;
            _MillisecondsDelay = millisecondsDelay;
        }

        public string JobName { get; private set; }
        public JobStatus JobStatus { get; set; }

        public async Task<JobExecDetail> ExcecuteAsync()
        {
            await Task.Delay(_MillisecondsDelay);
            Console.WriteLine("I am fired at "+DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return new JobExecDetail(JobExecResult.Success, JobName);
        }

    }
}
