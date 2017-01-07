using System;
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
    }
}

