namespace EasyScheduler.Tiny
{
    public interface IJob
    {
        string JobName { get; }
        void Excecute();
    }
}