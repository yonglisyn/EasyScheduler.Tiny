using System.Threading.Tasks;

namespace EasyScheduler.Tiny.Core
{
    public interface IJob
    {
        string JobName { get; }
        Task<JobExcecutionResult> ExcecuteAsync();
    }
}