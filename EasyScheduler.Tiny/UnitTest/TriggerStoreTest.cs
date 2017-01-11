using System;
using System.Collections.Generic;
using EasyScheduler.Tiny;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class TriggerStoreTest
    {
        [Test]
        public void TryGetTriggersToBeFired_ShouldReturnFalse_IfNoTrigger()
        {
            var target= new TriggerStore();
            List<ITrigger> toBeFired;
            FetchCycle fetchCycle = new FetchCycle(new DateTime(2016, 1, 1), new TimeSpan(30,0,0,0));
            var actual = target.TryGetTriggersToBeFired(out toBeFired, fetchCycle);

            Assert.AreEqual(false,actual);
            Assert.AreEqual(0,toBeFired.Count);
        }

        [Test]
        public void TryGetTriggersToBeFired_ShouldReturnTure_AndGetExpectedTriggers_AndUpdateTrigger()
        {
            var target= new TriggerStore();
            string jobName1="TestJob1";
            var triggerInRangeMoq = new CronTrigger(jobName1,"0 0 0 2 1 * 2016");
            DateTime nextFireTime = triggerInRangeMoq.GetNextFireTime(new DateTime(2016, 1, 1, 0, 0, 0));

            string jobName2 ="TestJob2";
            var triggerNotInRangeMoq = new CronTrigger(jobName2, "0 0 0 2 1 * 2015");
            target.TryAdd(triggerNotInRangeMoq);
            target.TryAdd(triggerInRangeMoq);

            List<ITrigger> toBeFired;
            FetchCycle fetchCycle = new FetchCycle(new DateTime(2016, 1, 1,0,0,0), new TimeSpan(30, 0, 0, 0));
            var actual = target.TryGetTriggersToBeFired(out toBeFired, fetchCycle);

            Assert.AreEqual(true,actual);
            Assert.AreEqual(1,toBeFired.Count);
            var actualTrigger = target.GetTriggerBy(jobName1);
            Assert.AreEqual(nextFireTime, actualTrigger.CurrentFireTime);
        }
    }
}
