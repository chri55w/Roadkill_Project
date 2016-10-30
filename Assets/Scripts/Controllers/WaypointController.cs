using System;
using System.Collections.Generic;
using UnityEngine;

using Objects;
using Events;

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
            SetterCollider.OnDriverTriggerExit += SetterCollider_OnDriverTriggerExit;
            UnSetterCollider.OnDriverTriggerExit += UnSetterCollider_OnDriverTriggerExit;
        }

        private void UnSetterCollider_OnDriverTriggerExit(DriverTriggerExitEventArgs e_EventArgs)
        {
            if (OnWaypointSetUnset != null)
            {
                OnWaypointSetUnset(new SetUnsetWaypointCollisionEventArgs(e_EventArgs.e_Driver, this, SetUnsetWayPointType.Unset));
            }
        }

        private void SetterCollider_OnDriverTriggerExit(DriverTriggerExitEventArgs e_EventArgs)
        {
            if (OnWaypointSetUnset != null)
                OnWaypointSetUnset(new SetUnsetWaypointCollisionEventArgs(e_EventArgs.e_Driver, this, SetUnsetWayPointType.Set));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
