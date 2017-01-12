using System;
using System.Threading;
using System.Threading.Tasks;
using EasyScheduler.Tiny;
using EasyScheduler.Tiny.Core;
using EasyScheduler.Tiny.Core.Exceptions;
using Moq;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class CronSchedulerTest
    {
        [Test]
        public void Dummy()
        {
            var maxNextFireTime = DateTime.Now + new TimeSpan(0, 1, 0);
            var minNextFireTime = DateTime.Now;
            for (int i = 0; i < 3; i++)
            {
                minNextFireTime = maxNextFireTime;
                maxNextFireTime = maxNextFireTime + new TimeSpan(0, 1, 0);
                Console.WriteLine(minNextFireTime);
                Console.WriteLine(maxNextFireTime);
            }
        }

        [Test]
        public void Dummy2()
        {
           Console.WriteLine(DateTime.Now);
            Thread.Sleep(100);
            Main();
        }

        private void Main()
        {
            var thread1 = new Thread(ThreadRunnerOne);
            var thread2 = new Thread(ThreadRunnerTwo);
            thread1.Start();
            thread2.Start();
        }

        private void ThreadRunnerTwo()
        {
            Console.WriteLine("ThreadRunnerTwo: "+DateTime.Now);
        }

        private void ThreadRunnerOne()
        {
            Console.WriteLine("ThreadRunnerOne: " + DateTime.Now);
            Task.Factory.StartNew(ThreadRunnerTwo,TaskCreationOptions.LongRunning);

        }

        [Test]
        public void Schedule_ShouldThrowException_IfTriggerJobNameNotMatch()
        {
            var jobMock = new Mock<IJob>();
            jobMock.Setup(x=>x.JobName).Returns("JobName");
            var triggerMock = new Mock<ITrigger>();
            triggerMock.Setup(x=>x.JobName).Returns("DiffJobName");
            var target = new CronScheduler();
            Assert.Throws<EasySchedulerException>(() => target.Schedule(jobMock.Object, triggerMock.Object));
        }

        [Test]
        public void Schedule_ShouldThrowException_IfTriggerJobNameNotMatch_CaseInsensitive()
        {
            var jobMock = new Mock<IJob>();
            jobMock.Setup(x=>x.JobName).Returns("JobName");
            var triggerMock = new Mock<ITrigger>();
            triggerMock.Setup(x=>x.JobName).Returns("jobName");
            var target = new CronScheduler();
            Assert.DoesNotThrow(() => target.Schedule(jobMock.Object, triggerMock.Object)); 
        }
    }
}

