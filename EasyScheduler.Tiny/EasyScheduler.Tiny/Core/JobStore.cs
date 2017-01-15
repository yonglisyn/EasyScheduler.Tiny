using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EasyScheduler.Tiny.Core
{
    internal static class JobStore
    {
        private static ConcurrentDictionary<string,IJob> _Jobs= new ConcurrentDictionary<string, IJob>();

        public static IJob TryGet(string jobName)
        {
            IJob job;
            _Jobs.TryGetValue(jobName,out job);
            return job;
        }

        public static void TryAdd(IJob job)
        {
            _Jobs.TryAdd(job.JobName, job);
        }

        public static void Reset()
        {
            _Jobs.Clear();
        }

        public static List<IJob> GetJobsToBeExcuted(List<string> triggersToBeFired)
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