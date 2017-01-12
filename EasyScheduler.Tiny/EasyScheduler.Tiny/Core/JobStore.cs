using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny.Core
{
    internal class JobStore
    {
        private ConcurrentBag<IJob> _Jobs;

        internal JobStore()
        {
            _Jobs = new ConcurrentBag<IJob>();
        }

        public IJob Get(string jobName)
        {
            return _Jobs.FirstOrDefault(x => x.JobName == jobName);
        }

        public void Add(IJob job)
        {
            _Jobs.Add(job);
        }

        public List<IJob> GetJobsToBeExcuted(List<ITrigger> triggersToBeFired)
        {
            return _Jobs.Where(x => triggersToBeFired.Exists(y => y.JobName == x.JobName)).ToList();
        }
    }
}