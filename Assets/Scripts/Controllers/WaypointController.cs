using System;
using System.Collections.Generic;
using UnityEngine;

using Objects;

namespace Controllers
{
    public delegate void WaypointSetUnset(SetUnsetWaypointCollisionEventArgs e_EventArgs);

    public class WaypointController : MonoBehaviour
    {
        public int ID;

        public List<WaypointController> NextWaypoints;

        public WaypointCollider SetterCollider;
        public WaypointCollider UnSetterCollider;

        public event WaypointSetUnset OnWaypointSetUnset;

        // Use this for initialization
        void Start()
        {
            // Set Up Event Listeners
            SetterCollider.OnPlayerTriggerExit += SetterCollider_OnPlayerTriggerExit;
            UnSetterCollider.OnPlayerTriggerExit += UnSetterCollider_OnPlayerTriggerExit;
        }

        private void UnSetterCollider_OnPlayerTriggerExit(PlayerTriggerExitEventArgs e_EventArgs)
        {
            if (OnWaypointSetUnset != null)
            {
                OnWaypointSetUnset(new SetUnsetWaypointCollisionEventArgs(e_EventArgs.e_Player, this, SetUnsetWayPointType.Unset));
            }
        }

        private void SetterCollider_OnPlayerTriggerExit(PlayerTriggerExitEventArgs e_EventArgs)
        {
            if (OnWaypointSetUnset != null)
                OnWaypointSetUnset(new SetUnsetWaypointCollisionEventArgs(e_EventArgs.e_Player, this, SetUnsetWayPointType.Set));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
