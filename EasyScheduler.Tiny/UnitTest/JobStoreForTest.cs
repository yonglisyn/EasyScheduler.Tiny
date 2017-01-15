using EasyScheduler.Tiny.Core;

namespace UnitTest
{
    internal class JobStoreForTest:JobStore
    {
        public void ResetJobStore()
        {
            Reset();
        }
    }
}
