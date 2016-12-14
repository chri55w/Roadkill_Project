using UnityEngine;
using System.Collections;

using Objects;
using Managers;

namespace Controllers
{
    public class PlayerController : Driver
    {
        public string ControllerID;

        new void Start()
        {
            base.Start();
            Debug.Log("Player start");
        }

        void Update()
        {
            if (DeathCheck() == false)
            {
                Kart.GetComponent<KartController>().Move(Input.GetAxis(ControllerID + "Vertical"), Input.GetAxis(ControllerID + "Horizontal"));

                if (Input.GetButtonDown(ControllerID + "FlipCamera"))
                    GetComponentInChildren<CameraController>().FlipCamera();
                if (Input.GetButtonUp(ControllerID + "FlipCamera"))
                    GetComponentInChildren<CameraController>().FlipCamera();
                if ((Input.GetButtonDown(ControllerID + "UsePickup")))
                {
                    CurrentPickup.GetComponent<Pickup>().UsePickup(gameObject);
                    if (CurrentPickup.GetComponent<Pickup>().PickupUses <= 0)
                        CurrentPickup.GetComponent<Pickup>().DeletePickup(gameObject);
                }
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