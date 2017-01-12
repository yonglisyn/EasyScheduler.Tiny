using System;

namespace EasyScheduler.Tiny
{
    //todo
    [Obsolete("No place yet, can try delete at the end")]
    public class DateTimeRange
    {
        private readonly DateTime _From;
        private readonly DateTime _To;

        public DateTime From { get { return _From; } }
        public DateTime To { get { return _To; } }

        public DateTimeRange(DateTime from, DateTime to)
        {
            if (DateTime.Compare(from, to) > 0)
            {
                throw new Exception("DateTimeRange: From larger than To date time");
            }
            _From = from;
            _To = to;
        }
    }
}