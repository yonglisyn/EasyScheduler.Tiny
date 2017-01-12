using System;

namespace EasyScheduler.Tiny.Core.Settings
{
    public class TaskDeliveryManagerSetting
    {
        private readonly TimeSpan _TaskDeliveryIdleCycle;

        private TaskDeliveryManagerSetting(TimeSpan taskDeliveryIdleCycle)
        {
            _TaskDeliveryIdleCycle = taskDeliveryIdleCycle;
        }

        public TimeSpan TaskDeliveryIdleCycle { get { return _TaskDeliveryIdleCycle; } }

        public static TaskDeliveryManagerSetting Default()
        {
            return new TaskDeliveryManagerSetting(new TimeSpan(0,0,0,0,10));
        }
    }
}