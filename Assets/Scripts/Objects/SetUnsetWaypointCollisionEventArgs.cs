using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Controllers;

namespace Objects
{
    public enum SetUnsetWayPointType { Set=0, Unset=1 };

    public class SetUnsetWaypointCollisionEventArgs
    {
        public Player e_Player;
        public WaypointController e_Waypoint;
        public SetUnsetWayPointType e_Type;

        public SetUnsetWaypointCollisionEventArgs(Player p_Player, WaypointController p_Waypoint, SetUnsetWayPointType p_Type)
        {
            e_Player = p_Player;
            e_Waypoint = p_Waypoint;
            e_Type = p_Type;
        }
    }
}
