using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Objects;

namespace Controllers
{
    public class AIController : Driver
    {
        public BezierSpline CenterPath;
        float ClosestTimePointOnSpline = 0f;

        private float m_PickupTimer;
        public Transform m_AimRaycastOrigin;

        public bool m_CheckingIfStuck = false;

        new void Start()
        {
            base.Start();
            m_AimRaycastOrigin = Kart.transform.Find("Raycast Points/Kart Front");
        }

        void Update()
        {
            if (m_CheckingIfStuck == false && m_RaceManager.RaceStarted == true && m_RaceManager.RaceFinished == false)
                StartCoroutine(StuckCheck());
        }

        void FixedUpdate()
        {
            if (!Active || IsFinished)
                return;

            int l_SplineDetail = CenterPath.MeshDetailLevel;

            Vector3 CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            ClosestTimePointOnSpline = CenterPath.GetClosestTimePointOnSpline(l_SplineDetail, CurrentPosition);
            
            float TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / l_SplineDetail) * 2)) % 1;

            Vector3 PointAhead = CenterPath.GetPoint(TimePointAhead);

            int increment = 1;

            while (Vector3.Distance(PointAhead, CurrentPosition) < 10)
            {
                TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / l_SplineDetail) * (2 + increment))) % 1;

                PointAhead = CenterPath.GetPoint(TimePointAhead);

                increment++;
            }

            Vector3 heading = PointAhead - CurrentPosition;

            float l_ForwardDot = Vector3.Dot(heading.normalized, Kart.transform.forward);

            float l_TurningDot = Vector3.Dot(heading.normalized, Kart.transform.right);

            Character.GetComponent<Character>().UpdateHorizontalInput(l_TurningDot);

            Kart.GetComponent<KartController>().Move(l_ForwardDot, l_TurningDot);

            // Does AI have a pickup at this point in time
            if ((m_PickupTimer <= 0) && (CurrentPickup != null))
            {
                float UsePickupChance = Random.Range(0, 10);
                if(UsePickupChance >= 5)
                {
                    Pickup l_LocalPickup = CurrentPickup.GetComponent<Pickup>();
                    switch (l_LocalPickup.PickupID)
                    {
                        case Pickup.e_PickupID.SPEED_BOOST:
                            // If not approaching corner
                            // Get a point 5-10 units ahead
                            // Compare the angle between current direction
                            // And direction to far point
                            // If angle is low enough use boost
                            Vector3 l_CurrentPoint = GetPointAhead(0);
                            Vector3 l_NextPoint = GetPointAhead(1);
                            Vector3 l_FuturePoint = GetPointAhead(7);

                            Vector3 l_NextPointDistance = l_NextPoint - l_CurrentPoint;
                            Vector3 l_FuturePointDistance = l_FuturePoint - l_CurrentPoint;

                            float l_Angle = Vector3.Angle(l_NextPointDistance, l_FuturePointDistance);

                            if (l_Angle <= 20.0f)
                                l_LocalPickup.UsePickup(gameObject);
                            break;
                        case Pickup.e_PickupID.LAND_MINE:
                            // Kart must be considered on the ground to be used
                            if(Kart.GetComponent<KartController>().WheelsGrounded() == 4)
                                l_LocalPickup.UsePickup(gameObject);
                            break;
                        case Pickup.e_PickupID.THROWING_AXE:
                            // Do I have a target
                            RaycastHit l_Hit = new RaycastHit();
                            if (Physics.SphereCast(m_AimRaycastOrigin.position, 7.5f, Kart.transform.forward, out l_Hit, 15.0f))                                                  
                                if (l_Hit.transform.parent.name.Contains("Driver"))                                
                                    l_LocalPickup.UsePickup(gameObject);                     
                            break;
                        case Pickup.e_PickupID.SHIELD:
                            //TODO: Is something incoming
                            l_LocalPickup.UsePickup(gameObject);
                            break;
                        case Pickup.e_PickupID.BLOOD_SLICK:
                            // Kart must be considered on the ground to be used
                            if (Kart.GetComponent<KartController>().WheelsGrounded() == 4)
                                l_LocalPickup.UsePickup(gameObject);
                            break;
                    }

                    if (l_LocalPickup.PickupUses <= 0)
                        l_LocalPickup.DeletePickup(gameObject);
                }

                SetRandomTime(1.0f, 5.0f);
            }
            else
                m_PickupTimer -= Time.deltaTime;
        }

        public void SwitchSpline(List<BezierSpline> p_SplineOptions)
        {
            int l_RandomIndex = Random.Range((int)0, (int)p_SplineOptions.Count);

            CenterPath = p_SplineOptions[l_RandomIndex];
        }

        private IEnumerator StuckCheck()
        {
            Vector3 l_CurrentPosition = Kart.transform.position;
            m_CheckingIfStuck = true;
            yield return new WaitForSeconds(2);
            if (l_CurrentPosition == Kart.transform.position) 
            {
                Kart.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Vector3 l_RespawnPoint = GetPointBehind(1);
                l_RespawnPoint.y += 0.5f;
                // Change to Lerp? - wont pass through waypoints otherwise
                Kart.transform.position = l_RespawnPoint;
                FaceForwards(l_RespawnPoint);
            }
            m_CheckingIfStuck = false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            // To draw position of AI drive point
            /* 
            int l_SplineDetail = CenterPath.MeshDetailLevel;

            Vector3 CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            ClosestTimePointOnSpline = CenterPath.GetClosestTimePointOnSpline(l_SplineDetail, CurrentPosition);

            float TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / l_SplineDetail) * 2)) % 1;

            Vector3 l_CurrentPoint = GetPointAhead(0);
            Vector3 l_NextPoint = GetPointAhead(1);
            Vector3 l_NextPointDistance = l_NextPoint - l_CurrentPoint;

            Vector3 l_FuturePoint = GetPointAhead(7);
            Vector3 l_FuturePointDistance = l_FuturePoint - l_CurrentPoint;

            Gizmos.DrawLine(l_CurrentPoint, l_NextPoint);
            Gizmos.DrawWireSphere(l_NextPoint, 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(l_CurrentPoint, l_FuturePoint);
            Gizmos.DrawWireSphere(l_FuturePoint, 0.5f);

            Vector3 PointAheadDistance = l_NextPoint - l_CurrentPoint;
            Vector3 PointFarAheadDistance = l_FuturePoint - l_CurrentPoint;
            

            float angle = Vector3.Angle(PointAheadDistance, PointFarAheadDistance);            
            Debug.Log(angle
            */
        }

        private void SetRandomTime(float p_MinTime, float p_MaxTime)
        {
            m_PickupTimer = Random.Range(p_MinTime, p_MaxTime);
        }
    }
}
