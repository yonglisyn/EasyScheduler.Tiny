using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny;
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
        public void Start_WillRunTheMainLoopOfExecutingJobs_BasedOnTriggers()
        {
            IJob job = new SimpleJob(typeof(SimpleJob).Name,"Trigger");
            var jobNormalMoq = new Mock<IJob>();
            jobNormalMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobNormalMoq.Setup(x => x.ExcecuteAsync());
            IJob jobNormal = jobNormalMoq.Object;
            string cronExpression = "0/10 * * * * * *";
            ITrigger tigger = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
                target.Start();
                target.Schedule(jobNormal, tigger);
            Thread.Sleep(new TimeSpan(0,0,20));
            jobNormalMoq.Verify(x=>x.ExcecuteAsync(),Times.Exactly(1));
        }

        [Test]
        public void OneJobThrowException_Run_ShouldContinue()
        {
            var jobNormalMoq = new Mock<IJob>();
            jobNormalMoq.SetupGet(x => x.JobName).Returns("SimpleJob");
            jobNormalMoq.Setup(x => x.ExcecuteAsync());
            IJob jobNormal = jobNormalMoq.Object;
            IJob jobException = new SimpleJobThrowException(typeof(SimpleJobThrowException).ToString());
            string cronExpression = "0/3 * * * * * *";
            ITrigger tigger = new CronTrigger("SimpleJob", cronExpression);
            ITrigger tiggerNormal = new CronTrigger("SimpleJob", cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            target.Schedule(jobException, tigger);
            target.Schedule(jobNormal, tiggerNormal);
            Thread.Sleep(new TimeSpan(0,1,0));
            jobNormalMoq.Verify(x=>x.ExcecuteAsync(),Times.Once);
            target.Stop();
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
        public bool ReadyToFire { get; set; }
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
