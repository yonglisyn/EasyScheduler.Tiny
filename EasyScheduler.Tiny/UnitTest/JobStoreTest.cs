using System;
using System.Globalization;
using System.Threading.Tasks;
using EasyScheduler.Tiny.Core;
using EasyScheduler.Tiny.Core.EnumsConstants;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class JobStoreTest
    {
        [TearDown]
        public void TearDown()
        {
            JobStore.Reset();
        }

        [Test]
        public void TryUpdateJobStatus_ShouldBeSuccessful()
        {
            var jobName = "Test";
            var job = new SimpleJob(jobName);
            JobStore.TryAdd(job);
            JobStore.TryUpdateJobStatus(jobName,JobStatus.Running);
            var actual = JobStore.TryGet(jobName);
            Assert.AreEqual(JobStatus.Running,actual.JobStatus);
        }

        public class SimpleJob : IJob
        {
            public SimpleJob(string jobName, JobStatus status = JobStatus.Idle)
            {
                JobName = jobName;
                JobStatus = status;
            }

            public string JobName { get; private set; }
            public JobStatus JobStatus { get; set; }

            public async Task<JobExecDetail> ExcecuteAsync()
            {
                await Task.Delay(10);
                Console.WriteLine("I am fired at " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                return new JobExecDetail(JobExecResult.Success, JobName);
            }

        }
    }
}
