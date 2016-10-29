using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Controllers;

namespace Objects
{
    public class PlayerTriggerExitEventArgs : EventArgs
    {
        public Player e_Player;
        public DateTime e_Timestamp;

        public PlayerTriggerExitEventArgs(Player p_Player, DateTime p_Timestamp) 
        {
            e_Player = p_Player;
            e_Timestamp = p_Timestamp;
        }
    }
}
