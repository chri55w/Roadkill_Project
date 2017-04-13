using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    public class Throwable : MonoBehaviour
    {
        public int damage = 1;
        public float ThrowSpeed = 100f;
        public float Lifetime = 5f;
        public float RotateSpeed = 20f;
        public float power = 10.0f;

        public Vector3 ThrowDirection;
        public Transform PivotPoint;
        public List<MeshRenderer> m_MeshRenderers = new List<MeshRenderer>();

        private int m_TrackingLevel = 0;
        public BezierSpline Track;
        public float ClosestTimePointOnSpline;
        private bool m_TargetFound = false;

        public BoxCollider m_BoxCollider;
        public BoxCollider m_TriggerCollider;
        private Rigidbody m_Rigidbody;
        private string m_ParentName;
        private Vector3 m_CurrentDirection;

        void OnEnable()
        {
            //Gather/set any variables
            m_Rigidbody = GetComponent<Rigidbody>();
            //Need to grab both boxcolliders
            m_TriggerCollider = GetComponent<BoxCollider>();
            
            //ThrowDirection = transform.forward;
            
            //This is for placeholder axe, will need to change for final axe, most likely
            foreach (MeshRenderer MR in m_MeshRenderers)
                MR.enabled = true;

            //m_BoxCollider.enabled = true;
            m_TriggerCollider.enabled = true;
            m_Rigidbody.velocity = transform.parent.GetComponentInChildren<Rigidbody>().velocity;
            m_Rigidbody.AddForce((ThrowDirection.normalized * ThrowSpeed), ForceMode.Impulse);
            //m_CurrentDirection = ThrowDirection;
            m_ParentName = transform.parent.name;
        }

        // Update is called once per frame
        void Update()
        { 
            if (Lifetime <= 0)
                Destroy(gameObject);
            else
                Lifetime -= Time.deltaTime;

            transform.RotateAround(PivotPoint.position, transform.right, RotateSpeed * Time.deltaTime);
        }

        void FixedUpdate()
        {
            // Check to see if there is a target infront of me
            // If yes head towards them
            // else head in current direction
            if (m_TrackingLevel == 2)
            {
                Vector3 l_NearestKartPosition = Vector3.zero;
                float l_NearestKartDistance = 10.0f;

                Collider[] l_Colliders = Physics.OverlapSphere(transform.position, 7.5f);

                foreach (Collider l_Hit in l_Colliders)
                {
                    if (l_Hit.name.Contains("Driver"))
                    {
                        m_TargetFound = true;
                        float l_KartDistance = Vector3.Distance(l_Hit.transform.position, transform.position);
                        if (l_KartDistance < l_NearestKartDistance)
                            l_NearestKartPosition = l_Hit.transform.position;
                    }
                    else
                    {
                        m_TargetFound = false;
                    }
                }

                if (m_TargetFound == false)
                {
                    int l_SplineDetail = Track.MeshDetailLevel;

                    ClosestTimePointOnSpline = Track.GetClosestTimePointOnSpline(l_SplineDetail, transform.position);

                    float TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / l_SplineDetail) * 5)) % 1;

                    Vector3 PointAhead = Track.GetPoint(TimePointAhead);

                    Vector3 l_NewDirection = PointAhead - transform.position;
                    float l_Hope = Vector3.Dot(l_NewDirection.normalized, m_CurrentDirection);
                    m_CurrentDirection = l_NewDirection;
                    //float l_Speed = m_Rigidbody.velocity.magnitude;
                    //Debug.Log(l_Speed);
                    //m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.AddForce((l_NewDirection.normalized * l_Hope), ForceMode.Impulse);
                }
                else
                {
                    Vector3 l_NewDirection = l_NearestKartPosition - transform.position;
                    m_Rigidbody.AddForce((l_NewDirection.normalized * ThrowSpeed), ForceMode.Acceleration);
                }
            }
            else if (m_TrackingLevel == 1)
            {
                RaycastHit l_Hit = new RaycastHit();
                //if (Physics.SphereCast(transform.position, 10f, ThrowDirection, out l_Hit, 20.0f))
                if(Physics.SphereCast(new Ray(transform.position, ThrowDirection), 10.0f, out l_Hit, 20.0f))
                {
                    if (l_Hit.transform.root.name.Contains("Driver"))
                    {
                        Vector3 l_NewDirection = l_Hit.transform.position - transform.position;
                        m_Rigidbody.AddForce((l_NewDirection.normalized * ThrowSpeed), ForceMode.Impulse);
                    }
                }
            }
        }

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            string l_ObjectName = p_OtherCollider.transform.root.name;
            if (l_ObjectName.Contains("Driver") && !l_ObjectName.Equals(m_ParentName))
            {
                p_OtherCollider.transform.GetComponentInParent<Driver>().TakeDamage(damage);
                p_OtherCollider.GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
                p_OtherCollider.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * power, ForceMode.VelocityChange);
                Destroy(gameObject);
            }
        }

        public void EnableTracking(int p_level)
        {
            m_TrackingLevel = p_level;
        }

        public void AssignTrack(BezierSpline p_Track)
        {
            Track = p_Track;
        }

        public void SwitchSpline(List<BezierSpline> p_SplineOptions)
        {
            // Need to make sure that the switcher does not have a shortcut set to 0
            Track = p_SplineOptions[0];
        }
    }
}
