using UnityEngine;

using Controllers;

namespace Objects
{
    public class AIController : Driver
    {
        void FixedUpdate()
        {
            WaypointController l_NextWaypoint = m_RaceManager.GetDriversNextWaypoint(Name);

            Vector3 NextWaypointPosition = new Vector3(l_NextWaypoint.transform.position.x, Kart.transform.position.y, l_NextWaypoint.transform.position.z);
            Vector3 CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            //Debug.DrawLine(CurrentPosition, NextWaypointPosition);

            Vector3 heading = NextWaypointPosition - CurrentPosition;

            float l_ForwardDot = Vector3.Dot(heading.normalized, Kart.transform.forward);

            float l_TurningDot = Vector3.Dot(heading.normalized, Kart.transform.right);

            //Debug.Log(l_NextWaypoint.name + " - " + l_ForwardDot + " - " + l_TurningDot);

            Kart.GetComponent<KartController>().Move(l_ForwardDot, l_TurningDot);
        }
    }
}
