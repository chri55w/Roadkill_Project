using UnityEngine;
using System.Collections.Generic;

using Objects;

namespace Controllers
{
    public class KartController : MonoBehaviour
    {
        public HoverWheel FrontLeftHoverWheel;
        public HoverWheel FrontRightHoverWheel;
        public HoverWheel RearLeftHoverWheel;
        public HoverWheel RearRightHoverWheel;

        //public float Force;
        public float MaxSpeed;
        public float MaxAccelerationForce;
        public float TurnSpeed;
        public float MaxRotationAngle;

        public Bezier AccelerationCurve = new Bezier(new Vector2(0, 1), new Vector2(0.7f, 1), new Vector2(1, 0.9f), new Vector2(1, 0));

        public Transform CenterOfGravity;
        public Transform PointOfAcceleration;

        public float SuspensionLength;
        public float UpwardForce;
        
        private Rigidbody m_Rigidbody;
        private List<HoverWheel> m_HoverWheels = new List<HoverWheel>();
        private float m_CurrentSpeed = 0f;
        private string m_DebugAcceleration;

        public void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();

            m_HoverWheels.Add(FrontLeftHoverWheel);
            m_HoverWheels.Add(FrontRightHoverWheel);
            m_HoverWheels.Add(RearLeftHoverWheel);
            m_HoverWheels.Add(RearRightHoverWheel);

            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
                l_HoverWheel.SuspensionLength = SuspensionLength;
            
            m_Rigidbody.centerOfMass = CenterOfGravity.localPosition;
        }
        
        public void FixedUpdate()
        {
            float l_VerticalInput = Input.GetAxis("Vertical");
            float l_HorizontalInput = Input.GetAxis("Horizontal");

            Vector3 l_AverageGroundNormal = new Vector3(0,0,0);

            //Suspension/Hover physics
            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
            {
                l_HoverWheel.Update();

                if (l_HoverWheel.GroundHitPoint.distance < l_HoverWheel.SuspensionLength)
                    m_Rigidbody.AddForceAtPosition(Vector3.up * UpwardForce * l_HoverWheel.UpForceModifier, l_HoverWheel.RaycastPosition.position, ForceMode.Acceleration);

                l_AverageGroundNormal += l_HoverWheel.GroundHitPoint.normal;
            }

            //Moving Forward
            Vector3 l_RearGroundHitAveragePoint = (RearLeftHoverWheel.GroundHitPoint.point + RearRightHoverWheel.GroundHitPoint.point) / 2;
            Vector3 l_FrontGroundHitAveragePoint = (FrontLeftHoverWheel.GroundHitPoint.point + FrontRightHoverWheel.GroundHitPoint.point) / 2;

            Vector3 l_DirectionOfAcceleration = (l_FrontGroundHitAveragePoint - l_RearGroundHitAveragePoint).normalized;

            float l_SpeedPercentage = (Mathf.Abs(m_Rigidbody.velocity.magnitude) / MaxSpeed);

            float l_AccelerationRate = AccelerationCurve.GetPoint(l_SpeedPercentage).y;

            float l_Acceleration = ((l_AccelerationRate * MaxAccelerationForce) * l_VerticalInput);

            //if(m_CurrentSpeed != MaxSpeed)
            //    m_CurrentSpeed += l_Acceleration * Time.deltaTime;

            float l_AccelerationForce =  m_Rigidbody.velocity.magnitude/ Time.deltaTime;
            m_DebugAcceleration = l_Acceleration.ToString("n2");

            Vector3 l_FowardForce = l_DirectionOfAcceleration * l_Acceleration /* m_Rigidbody.mass*/;        
            //Debug.Log(l_FowardForce);
            m_Rigidbody.AddForceAtPosition(l_FowardForce * Time.deltaTime, PointOfAcceleration.position);
            Debug.Log("Hello SourceTree");

            //Turning
            float l_SteeringAngle = l_HorizontalInput * MaxRotationAngle;

            l_AverageGroundNormal /= m_HoverWheels.Count;

            m_Rigidbody.AddTorque(l_AverageGroundNormal * l_SteeringAngle /*TurnSpeed*/ * Time.deltaTime);

            //Debug.Log(Vector3.Project(m_Rigidbody.velocity, transform.right));

            m_Rigidbody.AddForce(- (Vector3.Project(m_Rigidbody.velocity, transform.right) * 2), ForceMode.Acceleration);                   
        }

        public void OnGUI()
        {
            GUI.backgroundColor = Color.black;
            GUI.TextField(new Rect(10, 10, 100, 20), m_Rigidbody.velocity.ToString());
            GUI.TextField(new Rect(10, 30, 100, 20), m_DebugAcceleration);
        }
    }

    /*
    public class KartController : MonoBehaviour
    {
        public float MaxSpeed = 50.0f;
        public float MaxAcceleration = 1.5f;
        public Bezier AccelerationCurve = new Bezier(new Vector2(0, 1), new Vector2(0.7f, 1), new Vector2(1, 0.9f), new Vector2(1, 0));
        public float TurnSpeed = 180f;

        private float m_CurrentSpeed = 0f;
        private Rigidbody m_Rigidbody;
        private string m_VerticalAxisName;
        private float m_VerticalInputValue;
        private string m_HorizontalAxisName;
        private float m_HorizontalInputValue;
        
        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        
        private void Start()
        {
            m_VerticalAxisName = "Vertical";
            m_HorizontalAxisName = "Horizontal";
        }


        private void Update()
        {
            m_VerticalInputValue = Input.GetAxis(m_VerticalAxisName);
            m_HorizontalInputValue = Input.GetAxis(m_HorizontalAxisName);
        }

        private void FixedUpdate()
        {
            Move();
            Turn();
        }

        private void Move()
        {
            float l_SpeedPercentage = Mathf.Abs(m_CurrentSpeed) / MaxSpeed;

            float l_AccelerationRate = AccelerationCurve.GetPoint(l_SpeedPercentage).y;

            float l_Acceleration = (l_AccelerationRate * MaxAcceleration) * m_VerticalInputValue;

            m_CurrentSpeed += l_Acceleration * Time.deltaTime;

            Vector3 l_CalculatedMovement = transform.forward * (m_CurrentSpeed * Time.deltaTime);

            m_Rigidbody.MovePosition(m_Rigidbody.position + l_CalculatedMovement);
        }

        private void Turn()
        {
            float l_TurnAngle = m_HorizontalInputValue * TurnSpeed * Time.deltaTime;
            
            Quaternion l_TurnRotation = Quaternion.Euler(0f, l_TurnAngle, 0f);
            
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * l_TurnRotation);
        }
    }
    */
}
