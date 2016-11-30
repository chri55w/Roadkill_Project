using UnityEngine;
using System.Collections.Generic;

using Objects;

namespace Controllers
{
    public class AIController : Driver
    {
        public BezierSpline CenterPath;
        public int SplineDetail = 200;
        float ClosestTimePointOnSpline = 0f;
        
        void FixedUpdate()
        {
            Vector3 CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            ClosestTimePointOnSpline = CenterPath.GetClosestTimePointOnSpline(SplineDetail, CurrentPosition);
            
            float TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / SplineDetail) * 2)) % 1;

            Vector3 PointAhead = CenterPath.GetPoint(TimePointAhead);

            int increment = 1;

            while (Vector3.Distance(PointAhead, CurrentPosition) < 10)
            {
                TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / SplineDetail) * (2 + increment))) % 1;

                PointAhead = CenterPath.GetPoint(TimePointAhead);

                increment++;
            }

            Vector3 heading = PointAhead - CurrentPosition;

            float l_ForwardDot = Vector3.Dot(heading.normalized, Kart.transform.forward);

            float l_TurningDot = Vector3.Dot(heading.normalized, Kart.transform.right);

            Kart.GetComponent<KartController>().Move(l_ForwardDot, l_TurningDot);
        }

        public void SwitchSpline(List<BezierSpline> p_SplineOptions)
        {
            int l_RandomIndex = Random.Range((int)0, (int)p_SplineOptions.Count);

            Debug.Log("Range: 0 - " + (p_SplineOptions.Count ) + " || Random: " + l_RandomIndex);

            CenterPath = p_SplineOptions[l_RandomIndex];
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var transform = this.transform;

            float TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / SplineDetail) * 2)) % 1;

            Vector3 PointAhead = CenterPath.GetPoint(TimePointAhead);

            Gizmos.DrawWireSphere(PointAhead, 0.5f);
        }
    }
}
