using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Controllers;
using Objects;

namespace Events
{
    public class DriverTriggerExitEventArgs : EventArgs
    {
        public Driver e_Driver;
        public DateTime e_Timestamp;

        public DriverTriggerExitEventArgs(Driver p_Driver, DateTime p_Timestamp)
        {
            e_Driver = p_Driver;
            e_Timestamp = p_Timestamp;
        }
    }
}
