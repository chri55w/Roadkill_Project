using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour

    {
        // The target we are following
        public GameObject PlayerFollowing;
        [System.NonSerialized]
        public Transform ForwardTarget;
        [System.NonSerialized]
        public Transform RearTarget;
        public GameObject CurrentTarget;

        // The distance in the x-z plane to the target
        public float Distance = 10.0f;

        // the height we want the camera to be above the target
        public float Height = 5.0f;

        // Speed camera rotates
        public float RotationDamping;

        // Speed camera changes height - mainly for debug
        public float HeightDamping;

        void Start()
        {
            ForwardTarget = PlayerFollowing.transform.FindChild("CameraObjects/ForwardFacingFocalPoint").transform;
            RearTarget = PlayerFollowing.transform.FindChild("CameraObjects/BackwardFacingFocalPoint").transform;

            // Early out if we don't have a target
            if (!ForwardTarget || !RearTarget)
                return;

            CurrentTarget = ForwardTarget.gameObject;

            setSplitScreenPosition();
            // FacingFowards = true;

        }

        // Update is called once per frame
        void Update()
        {
            // Early out if we don't have a target
            //if (!ForwardTarget)
            //     return;

            if (Input.GetButtonDown("FlipCamera"))
            {
                CurrentTarget = RearTarget.gameObject;

                transform.position = CurrentTarget.transform.position;
                transform.position = Vector3.forward * Distance;
                transform.position = Vector3.up * Height;

                transform.LookAt(CurrentTarget.transform);
            }

            if (Input.GetButtonUp("FlipCamera"))
            {
                CurrentTarget = ForwardTarget.gameObject;

                transform.position = CurrentTarget.transform.position;
                transform.position -= Vector3.forward * Distance;
                transform.position = Vector3.up * Height;

                transform.LookAt(CurrentTarget.transform);
            }

            // Calculate the current rotation angles
            float l_WantedRotationAngle = CurrentTarget.transform.eulerAngles.y;
            float l_WantedHeight = CurrentTarget.transform.position.y + Height;

            float l_CurrentRotationAngle = transform.eulerAngles.y;
            float l_CurrentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            l_CurrentRotationAngle = Mathf.LerpAngle(l_CurrentRotationAngle, l_WantedRotationAngle, RotationDamping * Time.deltaTime);
            // Damp the height
            l_CurrentHeight = Mathf.Lerp(l_CurrentHeight, l_WantedHeight, HeightDamping * Time.deltaTime);
            // Convert the angle into a rotation
            Quaternion l_CurrentRotation = Quaternion.Euler(0, l_CurrentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = CurrentTarget.transform.position;
            transform.position -= l_CurrentRotation * Vector3.forward * Distance;
            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, l_CurrentHeight, transform.position.z);
            // Always look at the target
            transform.LookAt(CurrentTarget.transform);
        }

        void setSplitScreenPosition()
        {
            Camera thisCamera = GetComponent<Camera>();
            if (transform.parent.name.Contains("0"))
            {
                Rect camRect = new Rect(0f, 0.5f, 1f, 0.5f);
                thisCamera.rect = camRect;
            }
            else if (transform.parent.name.Contains("1"))
            {
                Rect camRect = new Rect(0f, 0f, 1f, 0.5f);
                thisCamera.rect = camRect;
            }
        }
    }
}