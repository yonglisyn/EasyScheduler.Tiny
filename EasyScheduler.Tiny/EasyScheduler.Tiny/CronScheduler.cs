using System.Collections.Concurrent;
using System.Linq;

namespace EasyScheduler.Tiny
{
    public class CronScheduler: IScheduler
    {
        private JobStore _JobStore;
        private TiggerStore _TiggerStore;

        public CronScheduler()
        {
            _JobStore = new JobStore();
            _TiggerStore = new TiggerStore();
        }

        public IJob GetJob(string jobName)
        {
            return _JobStore.Get(jobName);
        }

        public ITrigger GetTrigger(string triggerName)
        {
            throw new System.NotImplementedException();
        }

        public void Schedule(IJob job, ITrigger trigger)
        {
            _JobStore.Add(job);
        }

        public void Disable(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Enable(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }
    }

    public class TiggerStore
    {
    }

    public class JobStore
    {
        private static ConcurrentBag<IJob> _Jobs;

        static JobStore()
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
    }
}
