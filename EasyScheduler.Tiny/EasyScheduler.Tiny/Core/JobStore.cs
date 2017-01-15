using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny.Core
{
    internal class JobStore
    {
        private static ConcurrentDictionary<string,IJob> _Jobs;

        static JobStore()
        {
            _Jobs = new ConcurrentDictionary<string, IJob>();
        }

        public IJob TryGet(string jobName)
        {
            IJob job;
            _Jobs.TryGetValue(jobName,out job);
            return job;
        }

        public void TryAdd(IJob job)
        {
            _Jobs.TryAdd(job.JobName, job);
        }

        protected void Reset()
        {
            _Jobs.Clear();
        }

        public List<IJob> GetJobsToBeExcuted(List<string> triggersToBeFired)
        {
            return triggersToBeFired.Select(x =>
            {
                IJob job;
                _Jobs.TryGetValue(x, out job);
                return job;
            }).ToList();
        }
    }
}