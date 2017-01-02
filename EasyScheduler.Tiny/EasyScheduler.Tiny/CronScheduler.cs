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

        public ITrigger GetTrigger(string jobName)
        {
            return _TiggerStore.GetTriggerBy(jobName);
        }

        public void Schedule(IJob job, ITrigger trigger)
        {
            _JobStore.Add(job);
            _TiggerStore.Add(trigger);
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
}
