using UnityEngine;
using System.Collections;

public class SimpleCinematicCamera : MonoBehaviour
{
    public Objects.BezierSpline MovementSpline;
    public float PointsOnSpline = 200;
    public float PointsPerSecond = 10;
    public GameObject CameraTarget;

    private float time = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (MovementSpline != null)
        {
            if (time < 1.0f)
            {
                time += (PointsPerSecond / PointsOnSpline) * Time.deltaTime;

                Vector3 l_Position = MovementSpline.GetPoint(time);

                gameObject.transform.position = l_Position;
            }
        }
        if (CameraTarget != null)
            gameObject.transform.LookAt(CameraTarget.transform);
    }

    public bool IsFinished()
    {
        return Mathf.Floor(time) == 1.0f;
    }

    public void ResetTime()
    {
        time = 0.0f;
    }
}
