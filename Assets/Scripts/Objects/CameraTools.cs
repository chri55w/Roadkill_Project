using UnityEngine;
using System.Collections;

public class CameraTools : MonoBehaviour {

    [System.NonSerialized]
    public Camera CameraComponent;

    void Start()
    {
        CameraComponent = GetComponent<Camera>();
    }

    public void SetCameraScreenSize(Rect p_ScreenSize)
    {
        if (CameraComponent == null)
            CameraComponent = GetComponent<Camera>();

        CameraComponent.rect = p_ScreenSize;
    }
}
