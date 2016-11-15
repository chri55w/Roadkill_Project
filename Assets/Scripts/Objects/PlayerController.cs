using UnityEngine;
using System.Collections;

using Controllers;
using Managers;

namespace Objects
{
    public class PlayerController : Driver
    {
        public string ControllerID;
        
        void Update()
        {
            Kart.GetComponent<KartController>().Move(Input.GetAxis(ControllerID + "Vertical"), Input.GetAxis(ControllerID + "Horizontal"));

            if (Input.GetButtonDown(ControllerID + "FlipCamera"))
                GetComponentInChildren<CameraController>().FlipCamera();
            if (Input.GetButtonUp(ControllerID + "FlipCamera"))
                GetComponentInChildren<CameraController>().FlipCamera();
            if ((Input.GetButtonDown(ControllerID + "UsePickup")))
            {
                CurrentPickup.GetComponent<Pickup>().UsePickup(gameObject);
                Destroy(CurrentPickup);
            }
        }

        void OnGUI()
        {
            GUI.backgroundColor = Color.black;

            if (CurrentPickup)
            {
                GUI.TextField(new Rect(Screen.width - 300, 10, 200, 20), string.Format("Current Pickup Name: {0}", CurrentPickup.GetComponent<Pickup>().PickupName));
                GUI.TextField(new Rect(Screen.width - 300, 35, 200, 20), string.Format("Current Pickup Level: {0}", CurrentPickup.GetComponent<Pickup>().PickupLevel));
            }
        }

    }
}