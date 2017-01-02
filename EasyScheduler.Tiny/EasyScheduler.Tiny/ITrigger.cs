using System;

namespace EasyScheduler.Tiny
{
    public interface ITrigger
    {
        string JobName { get;}
        bool ReadyToFire { get; set; }
        DateTime FirstFireTime { get; }
        DateTime? LastFireTime { get; }
        DateTime GetNextFireTime();
    }
}