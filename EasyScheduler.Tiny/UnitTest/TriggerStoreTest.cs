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
            var actual = target.TryGetTriggersToBeFired(new DateTime(2016, 1, 1), new DateTime(2016, 2, 1),
                out toBeFired, DateTime.Now);

            Assert.AreEqual(false,actual);
            Assert.AreEqual(0,toBeFired.Count);
        }

        [Test]
        public void TryGetTriggersToBeFired_ShouldReturnTure_AndGetExpecteTriggers_AndUpdateTrigger()
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
            var actual = target.TryGetTriggersToBeFired(new DateTime(2016, 1, 1,0,0,0), new DateTime(2016, 2, 1),
                out toBeFired, new DateTime(2016, 1, 1, 0, 0, 0));

            Assert.AreEqual(true,actual);
            Assert.AreEqual(1,toBeFired.Count);
            var actualTrigger = target.GetTriggerBy(jobName1);
            Assert.AreEqual(false, actualTrigger.ReadyToFire);
            Assert.AreEqual(nextFireTime, actualTrigger.CurrentFireTime);
        }
    }
}
