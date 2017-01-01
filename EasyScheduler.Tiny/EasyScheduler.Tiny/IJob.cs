namespace EasyScheduler.Tiny
{
    public interface IJob
    {
        string TriggerName { get; }
        string Description { get; }
        JobFireHistory GetJobFireHistory(DateTimeRange range);
        void Excecute();
    }
}