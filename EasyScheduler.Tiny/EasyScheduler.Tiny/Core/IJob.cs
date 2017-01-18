using System.Threading.Tasks;
using EasyScheduler.Tiny.Core.EnumsConstants;

namespace EasyScheduler.Tiny.Core
{
    public interface IJob
    {
        string JobName { get; }
        JobStatus JobStatus { get; set; }
        Task<JobExecDetail> ExcecuteAsync();
    }
}