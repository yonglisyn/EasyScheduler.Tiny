using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public interface IJob
    {
        string JobName { get; }
        Task<JobExcecutionResult> Excecute();
    }

    public class JobExcecutionResult
    {
        public static JobExcecutionResult Success { get; set; }
    }
}