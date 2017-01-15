using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EasyScheduler.Tiny.Core.EnumsConstants;

namespace EasyScheduler.Tiny.Core
{
    internal static class JobStore
    {
        private static ConcurrentDictionary<string,IJob> _Jobs= new ConcurrentDictionary<string, IJob>();
        private static object _Locker = new object();

        public static IJob TryGet(string jobName)
        {
            IJob job;
            _Jobs.TryGetValue(jobName,out job);
            return job;
        }

        public static bool TryAdd(IJob job)
        {
            return _Jobs.TryAdd(job.JobName, job);
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

        public static bool TryUpdate(IJob job)
        {
            var oldJob = TryGet(job.JobName);
            return _Jobs.TryUpdate(job.JobName, job, oldJob);
        }

        public static void TryUpdateJobStatus(string jobName, JobStatus status)
        {
            var oldJob = TryGet(jobName);
            lock (oldJob)
            {
                oldJob.JobStatus = status;
            }
        }
    }
}