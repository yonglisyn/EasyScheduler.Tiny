using System;
using EasyScheduler.Tiny.Core.EnumsConstants;

namespace EasyScheduler.Tiny.Core
{
    public class JobNotificationCenter
    {
        public void NotifyJobSwitchStatus(JobStatus started, DateTime currentFireTime)
        {
            //todo need to be fire and forget to do not take up time to continue next loop
        }

        public void NotifyResult(JobExecDetail result)
        {
            return;
        }
    }
}