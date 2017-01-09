using System.Threading.Tasks;

namespace EasyScheduler.Tiny
{
    public interface IJob
    {
        string JobName { get; }
        Task<JobExcecutionResult> ExcecuteAsync();
    }

    public class JobExcecutionResult
    {
        public static JobExcecutionResult Success { get; set; }
    }
}