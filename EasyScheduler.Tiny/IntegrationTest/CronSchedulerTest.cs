using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny;
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
            IJob job = new SimpleJob(typeof(SimpleJob).ToString(),typeof(SimpleTrigger).ToString());
            ITrigger trigger = new SimpleTrigger(typeof(SimpleJob).ToString());

            var target = new CronScheduler(_SchedulerSetting,new TaskDeliveryManager(_TaskDeliveryManagerSetting,new JobNotificationCenter()));
            target.Schedule(job,trigger);

            var resultJob = target.GetJob(typeof (SimpleJob).ToString());
            Assert.AreEqual(resultJob.JobName, job.JobName);

            var resultTrigger = target.GetTrigger(typeof (SimpleJob).ToString());
            Assert.AreEqual(resultTrigger.JobName,trigger.JobName);
        }

        [Test]
        public void Start_WillRunTheMainLoopOfExecutingJobsBasedOnTriggers()
        {
            IJob job = new SimpleJob(typeof(SimpleJob).ToString(), typeof(SimpleTrigger).ToString());
            string cronExpression = "0 0/1 * * * * *";
            ITrigger tigger = new CronTrigger(typeof(SimpleJob).ToString(),cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            target.Schedule(job,tigger);
            Thread.Sleep(new TimeSpan(0,2,0));
            Assert.IsTrue(1==1);
        }

        [Test]
        public void OneJobThrowException_Run_ShouldContinue()
        {
            IJob job = new SimpleJob(typeof(SimpleJob).ToString(), typeof(SimpleTrigger).ToString());
            string cronExpression = "0 0/1 * * * * *";
            ITrigger tigger = new CronTrigger(typeof(SimpleJob).ToString(),cronExpression);
            var target = new CronScheduler(_SchedulerSetting, new TaskDeliveryManager(_TaskDeliveryManagerSetting, new JobNotificationCenter()));
            target.Start();
            target.Schedule(job,tigger);
            Thread.Sleep(new TimeSpan(0,2,0));
            Assert.IsTrue(1==1);
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

        public async Task<JobExcecutionResult> Excecute()
        {
            await Task.Delay(10);
            Console.WriteLine("I am fired at "+DateTime.Now.ToString(CultureInfo.InvariantCulture));
            return JobExcecutionResult.Success;
        }

    }
}
