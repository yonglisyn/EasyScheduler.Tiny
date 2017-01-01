using System;

namespace EasyScheduler.Tiny
{
    public interface ITrigger
    {
        string JobName { get; }
        DateTime StartFireTime { get; }
        DateTime EndFireTime { get; }
        DateTime GetNextFireTime();
    }
}