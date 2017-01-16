using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour

    {
        // The target we are following
        public GameObject DriverFollowing;

        [System.NonSerialized]
        public Transform ForwardTarget;
        [System.NonSerialized]
        public Transform RearTarget;
        public GameObject CurrentTarget;
        public bool FacingForward = true;
        [System.NonSerialized]
        public Camera CameraComponent;

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
            // Early out if we don't have a target
            ForwardTarget = DriverFollowing.transform.FindChild("CameraObjects/ForwardFacingFocalPoint").transform;
            RearTarget = DriverFollowing.transform.FindChild("CameraObjects/BackwardFacingFocalPoint").transform;

            // Early out if we can't find both front and rear targets
            if (!ForwardTarget || !RearTarget)
                return;

            CurrentTarget = ForwardTarget.gameObject;
            CameraComponent = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // Early out if we don't have a target
            if (!ForwardTarget)
                 return;

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

        public void SetCameraScreenSize(Rect p_ScreenSize)
        {
            if (CameraComponent == null)
                CameraComponent = GetComponent<Camera>();
            CameraComponent.rect = p_ScreenSize;
        }

        public void FlipCamera()
        {
            if (FacingForward)
            {
                CurrentTarget = RearTarget.gameObject;

                transform.position = CurrentTarget.transform.position;
                transform.position = Vector3.forward * Distance;
                transform.position = Vector3.up * Height;

                transform.LookAt(CurrentTarget.transform);
                FacingForward = false;
            }
            else
            {
                CurrentTarget = ForwardTarget.gameObject;

                transform.position = CurrentTarget.transform.position;
                transform.position -= Vector3.forward * Distance;
                transform.position = Vector3.up * Height;

                transform.LookAt(CurrentTarget.transform);
                FacingForward = true;
            }
        }
    }
}