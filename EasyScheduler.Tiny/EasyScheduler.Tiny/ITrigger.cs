using System;

namespace EasyScheduler.Tiny
{
    public interface ITrigger
    {
        string JobName { get; }
        string Description { get; }
        DateTime StartFireTime { get; }
        DateTime EndFireTime { get; }
        DateTime GetNextFireTime();
    }
}