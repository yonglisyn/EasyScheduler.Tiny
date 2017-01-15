using System;
using System.Collections.Generic;
using EasyScheduler.Tiny.Core;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture]
    public class TriggerStoreTest
    {
        [TearDown]
        public void TearDown()
        {
            TriggerStore.Reset();
        }


        [Test]
        public void TryGetTriggersToBeFired_ShouldReturnFalse_IfNoTrigger()
        {
            List<ITrigger> toBeFired;
            FetchCycle fetchCycle = new FetchCycle(new DateTime(2016, 1, 1), new TimeSpan(30,0,0,0));
            var actual = TriggerStore.TryGetTriggersToBeFired(out toBeFired, fetchCycle);

            Assert.AreEqual(false,actual);
            Assert.AreEqual(0,toBeFired.Count);
        }

        [Test]
        public void TryGetTriggersToBeFired_ShouldReturnTure_AndGetExpectedTriggers_AndUpdateTrigger()
        {
            string jobName1 = "TestJob1";
            var triggerInRangeMoq = new CronTrigger(jobName1, "0 0 0 2 1 * 2016");
            DateTime nextFireTime = triggerInRangeMoq.GetNextFireTime(new DateTime(2016, 1, 1, 0, 0, 0));

            string jobName2 = "TestJob2";
            var triggerNotInRangeMoq = new CronTrigger(jobName2, "0 0 0 2 1 * 2015");
            TriggerStore.TryAdd(triggerNotInRangeMoq);
            TriggerStore.TryAdd(triggerInRangeMoq);

            List<ITrigger> toBeFired;
            FetchCycle fetchCycle = new FetchCycle(new DateTime(2016, 1, 1, 0, 0, 0), new TimeSpan(30, 0, 0, 0));
            var actual = TriggerStore.TryGetTriggersToBeFired(out toBeFired, fetchCycle);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(1, toBeFired.Count);
            var actualTrigger = TriggerStore.GetTriggerBy(jobName1);
            Assert.AreEqual(nextFireTime, actualTrigger.CurrentFireTime);
        }

        [Test]
        public void GetTriggerBy_ShouldReturnNull_IfNoTriggerFound()
        {
            var actual = TriggerStore.GetTriggerBy("dummyName");
            Assert.IsNull(actual);
        }

        [Test]
        public void GetTriggerBy_ShouldReturnValidTrigger()
        {
            TriggerStore.TryAdd(new CronTrigger("DummyOne", "0 0 0 0 0 0 0"));
            var actual = TriggerStore.GetTriggerBy("DummyOne");
            Assert.AreEqual("DummyOne",actual.JobName);
        }
    }
}
