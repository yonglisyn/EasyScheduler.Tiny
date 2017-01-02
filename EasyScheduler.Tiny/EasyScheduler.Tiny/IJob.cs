namespace EasyScheduler.Tiny
{
    public interface IJob
    {
        string JobName { get; }
        JobExcecutionResult Excecute();
    }

    public class JobExcecutionResult
    {
        public static JobExcecutionResult Success { get; set; }
    }
}