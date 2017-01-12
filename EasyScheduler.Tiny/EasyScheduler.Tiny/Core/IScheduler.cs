namespace EasyScheduler.Tiny.Core
{
    public interface IScheduler
    {
        IJob GetJob(string jobName);
        ITrigger GetTrigger(string triggerName);
        void Schedule(IJob job, ITrigger trigger);
        void Disable(string jobName);
        void Enable(string jobName);
        void Delete(string jobName);
        void Start();
        void Stop();
        void Pause();
    }
}
