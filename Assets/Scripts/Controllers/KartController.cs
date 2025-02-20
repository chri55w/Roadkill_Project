﻿using UnityEngine;
using System.Collections.Generic;

using Objects;

namespace Controllers
{
    public class KartController : MonoBehaviour
    {
        public List<SkinnableObject> SkinnableKartPeices;

        //Hover Wheel Points
        public HoverWheel FrontLeftHoverWheel;
        public HoverWheel FrontRightHoverWheel;
        public HoverWheel RearLeftHoverWheel;
        public HoverWheel RearRightHoverWheel;
        
        //Speed & Turning Settings
        public float MaxSpeed;
        public float MaxAccelerationForce;
        public float MaxRotationAngle;
        public float TurnForce;
        public Bezier AccelerationCurve = new Bezier(new Vector2(0, 1), new Vector2(0.7f, 1), new Vector2(1, 0.9f), new Vector2(1, 0));

        public ParticleSystem BloodSpray;
        public ParticleSystem BloodBurst;

        //Force Application Transforms
        public Transform CenterOfGravity;
        public Transform PointOfAcceleration;

        //Hover Physics Settings
        public float SuspensionLength;
        public float UpwardForce;
        
        private Rigidbody m_Rigidbody;
        private List<HoverWheel> m_HoverWheels = new List<HoverWheel>();
        private Dictionary<string, string> DebugParameters = new Dictionary<string, string>();

        public void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();

            //            Add all Hover Wheels to a List for Quicker Processing 
            //                   then Setup the Suspension Length on all Wheels
            m_HoverWheels.Add(FrontLeftHoverWheel);
            m_HoverWheels.Add(FrontRightHoverWheel);
            m_HoverWheels.Add(RearLeftHoverWheel);
            m_HoverWheels.Add(RearRightHoverWheel);

            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
            {
                l_HoverWheel.SuspensionLength = SuspensionLength;
                l_HoverWheel.CalculateWheelCircumference();
            }

            //   Offset the Center of Gravity on the Rigidbody to help the kart
            //                                to always land the right side up.
            m_Rigidbody.centerOfMass = CenterOfGravity.localPosition;
        }
        
        public void Update()
        {
        }

        public void FixedUpdate()
        {
            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
                l_HoverWheel.Update(Time.fixedDeltaTime);

            // Suspension (Hover) physics - Applies a Force at each Hover Point 
            // to keep the Kart at the distance specified in 'SuspensionLength'
            //                           from the Ground below each Hover Point
            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
            {
                m_Rigidbody.AddForceAtPosition(l_HoverWheel.RaycastPosition.up * UpwardForce * l_HoverWheel.UpForceModifier, l_HoverWheel.RaycastPosition.position, ForceMode.Acceleration);
            }
        }

        public int WheelsGrounded()
        {
            int GroundedWheels = 0;

            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
                if (l_HoverWheel.GroundHitPoint.distance < 1.5f)
                    GroundedWheels++;

            return GroundedWheels;
        }
        
        public void Move(float p_VerticalInput, float p_HorizontalInput)
        {
            Vector3 l_AverageGroundNormal = new Vector3(0, 0, 0);
            
            foreach (HoverWheel l_HoverWheel in m_HoverWheels)
            {
                l_AverageGroundNormal += l_HoverWheel.GroundHitPoint.normal;
            }

            Vector3 l_LocalVelocity = transform.InverseTransformDirection(m_Rigidbody.velocity);

            float l_DistanceTravelled = l_LocalVelocity.z * Time.deltaTime;

            float l_SteeringAngle = p_HorizontalInput * MaxRotationAngle;

            //                Set the Wheel Mesh Rotations to Simulate Movement
            FrontLeftHoverWheel.SetWheelRotation(l_SteeringAngle, l_DistanceTravelled);
            FrontRightHoverWheel.SetWheelRotation(l_SteeringAngle, l_DistanceTravelled);
            RearLeftHoverWheel.SetWheelRotation(0, l_DistanceTravelled);
            RearRightHoverWheel.SetWheelRotation(0, l_DistanceTravelled);

            if (WheelsGrounded() < 3)
            {
                if (WheelsGrounded() <= 0)
                {
                    m_Rigidbody.angularDrag = 0.25f;
                    m_Rigidbody.drag = 0.25f;
                }

                // Kart not considered on the ground therefore it cannot change current movement
                return;
            }
            else
            {
                m_Rigidbody.angularDrag = 5f;
                m_Rigidbody.drag = 1f;
            }

            //Moving Forward
            Vector3 l_DirectionOfAcceleration = GetDirectionOfFowardMovement();
            
            //     Calculate the current percentage of the speed in a 0-1 value 
            //     and use it to get required accelleration force to be applied
            //                                                    to the object
            float l_SpeedPercentage = (Mathf.Abs(m_Rigidbody.velocity.magnitude) / MaxSpeed);

            float l_AccelerationRate = AccelerationCurve.GetPoint(l_SpeedPercentage).y;

            float l_Acceleration = ((l_AccelerationRate * MaxAccelerationForce) * p_VerticalInput);
            
            //                 Apply the acceleration force in the direction of 
            //                                  acceleration (ground direction)
            m_Rigidbody.AddForceAtPosition((l_DirectionOfAcceleration * l_Acceleration) * Time.deltaTime, PointOfAcceleration.position, ForceMode.Acceleration);

            //          Calculate Steering angle from Input and apply Torque to
            //                       the Kart using the Averaged Ground Normal.

            l_AverageGroundNormal /= m_HoverWheels.Count;

            m_Rigidbody.AddTorque(l_AverageGroundNormal * l_SteeringAngle * TurnForce * Time.deltaTime);

            //                Apply Anti-Slip Force to reduce sliding on Track.
            m_Rigidbody.AddForce(- (Vector3.Project(m_Rigidbody.velocity, transform.right) * 2), ForceMode.Acceleration);

            if (m_Rigidbody.velocity.magnitude < 0.5f)
                m_Rigidbody.velocity = new Vector3(0, 0, 0);
        }
        
        public Vector3 GetDirectionOfFowardMovement()
        {
            Vector3 l_RearGroundHitAveragePoint = (RearLeftHoverWheel.GroundHitPoint.point + RearRightHoverWheel.GroundHitPoint.point) / 2;
            Vector3 l_FrontGroundHitAveragePoint = (FrontLeftHoverWheel.GroundHitPoint.point + FrontRightHoverWheel.GroundHitPoint.point) / 2;

            Vector3 l_DirectionOfForwardMovement = (l_FrontGroundHitAveragePoint - l_RearGroundHitAveragePoint).normalized;

            return l_DirectionOfForwardMovement;
        }

        public void Boost(float p_BoostForce)
        {
            m_Rigidbody.AddForceAtPosition((GetDirectionOfFowardMovement() * p_BoostForce) * Time.deltaTime, PointOfAcceleration.position);
            Transform l_Thrusters = transform.Find("VFX/Thrusters");
            foreach (Transform l_Thruster in l_Thrusters.transform)
            {
                l_Thruster.gameObject.GetComponent<ParticleSystem>().Play(true);
            }
        }

        public void EmitBloodBurst()
        {
            BloodBurst.Play();
        }

        public void UpdateBloodSpray(int p_Health)
        {
            var l_Emission = BloodSpray.emission;

            if (p_Health == 3)
                l_Emission.rate = 0;
            else if (p_Health == 2)
                l_Emission.rate = 15;
            else if (p_Health == 1)
                l_Emission.rate = 35;
            else if (p_Health == 0)
                l_Emission.rate = 80;

            BloodSpray.Play();
        }

        public void ChangeKartSkin()
        {
            foreach (SkinnableObject l_Object in SkinnableKartPeices)
            {
                l_Object.NextSkin();
            }
        }

        public void ResetKartSkin()
        {
            foreach (SkinnableObject l_Object in SkinnableKartPeices)
            {
                l_Object.ResetSkin();
            }
        }

        public int GetMaterialSkinIndex()
        {
            return SkinnableKartPeices[0].MaterialIndex;
        }

        public void SetKartSkin(int p_MaterialIndex)
        {
            foreach (SkinnableObject l_Object in SkinnableKartPeices)
            {
                l_Object.SetSkin(p_MaterialIndex);
            }
        }

        public void SetRandomKartSkin( )
        {
            int l_MaterialCount = SkinnableKartPeices[0].Materials.Count;

            int l_Random = Random.Range(0, l_MaterialCount);

            SetKartSkin(l_Random);
        }
    }
}
