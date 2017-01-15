using EasyScheduler.Tiny.Core;

namespace UnitTest
{
    internal class TriggerStoreForTest:TriggerStore
    {
        public void ResetTrigger()
        {
            Reset();
        }
    }
}