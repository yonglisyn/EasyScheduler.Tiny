namespace EasyScheduler.Tiny
{
    public class CronScheduler: IScheduler
    {
        private JobStore _JobStore;
        private TiggerStore _TiggerStore;

        public IJob GetJob(string jobName)
        {
            throw new System.NotImplementedException();
        }

        public ITrigger GetTrigger(string triggerName)
        {
            throw new System.NotImplementedException();
        }

        public void Schedule(IJob job, ITrigger trigger)
        {
            throw new System.NotImplementedException();
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

    internal class TiggerStore
    {
    }

    internal class JobStore
    {
    }
}
