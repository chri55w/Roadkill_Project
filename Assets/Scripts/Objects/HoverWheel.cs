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
        public float WheelRadius;

        public HoverWheel()
        {
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
    }
}