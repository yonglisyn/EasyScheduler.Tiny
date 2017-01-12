using System;

namespace EasyScheduler.Tiny.Core
{
    public interface ITrigger
    {
        string JobName { get;}
        DateTime FirstFireTime { get; }
        DateTime? LastFireTime { get; }
        DateTime GetNextFireTime(DateTime baseValue);
        DateTime CurrentFireTime { get; set; }
    }
}