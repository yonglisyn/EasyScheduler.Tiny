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
        }
    }
}
