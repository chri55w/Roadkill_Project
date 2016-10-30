using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Controllers;
using Objects;

namespace Events
{
    public enum SetUnsetWayPointType { Set=0, Unset=1 };

    public class SetUnsetWaypointCollisionEventArgs : EventArgs
    {
        public Driver e_Driver;
        public WaypointController e_Waypoint;
        public SetUnsetWayPointType e_Type;

        public SetUnsetWaypointCollisionEventArgs(Driver p_Driver, WaypointController p_Waypoint, SetUnsetWayPointType p_Type)
        {
            e_Driver = p_Driver;
            e_Waypoint = p_Waypoint;
            e_Type = p_Type;
        }
    }
}
