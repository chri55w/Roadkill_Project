using UnityEngine;
using System.Collections;

namespace Objects
{
    [System.Serializable]
    public class HoverWheel
    {
        [System.NonSerialized]
        public float CompressionRatio;
        [System.NonSerialized]
        public RaycastHit GroundHitPoint;
        [System.NonSerialized]
        public float SuspensionLength;
        [System.NonSerialized]
        public float UpForceModifier;

        //Serialized Variables for Access in Editor
        public Transform RaycastPosition;
        public Transform WheelMeshTransform;
        [Range(0.1f, float.MaxValue)]
        public float WheelRadius;

        private float m_WheelCircumference;
        private float m_WheelSpinAngle;
        
        public void CalculateWheelCircumference()
        {
            m_WheelCircumference = 2 * Mathf.PI * WheelRadius;

            Debug.Log(m_WheelCircumference);
        }

        public void Update()
        {
            Physics.Raycast(RaycastPosition.position, -RaycastPosition.up, out GroundHitPoint);

            CompressionRatio = (GroundHitPoint.distance / SuspensionLength);

            CompressionRatio = Mathf.Clamp(CompressionRatio, 0, 1);

            if (GroundHitPoint.distance < SuspensionLength)
                UpForceModifier = 1.0f - (GroundHitPoint.distance / SuspensionLength);

            WheelMeshTransform.localPosition = new Vector3(WheelMeshTransform.localPosition.x, (-SuspensionLength * CompressionRatio) + WheelRadius, WheelMeshTransform.localPosition.z);
        }

        public void SetWheelRotation(float p_SteeringAngle, float p_WheelDistanceTravelled)
        {
            m_WheelSpinAngle = (m_WheelSpinAngle + ((p_WheelDistanceTravelled / m_WheelCircumference) * 360f)) % 360f;
            
            WheelMeshTransform.localEulerAngles = new Vector3(m_WheelSpinAngle, p_SteeringAngle, 0);
        }
    }
}