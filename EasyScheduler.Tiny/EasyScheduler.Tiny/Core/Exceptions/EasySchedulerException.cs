using System;

namespace EasyScheduler.Tiny.Core.Exceptions
{
    public class EasySchedulerException : Exception
    {
        public EasySchedulerException(string message):base(message)
        {
        }
    }
}