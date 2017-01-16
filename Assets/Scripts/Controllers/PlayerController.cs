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

        void FixedUpdate()
        {

        }

        void Update()
        {
            if (!Active)
                return;

            if (DeathCheck() == false)
            {
                Kart.GetComponent<KartController>().Move(Input.GetAxis(ControllerID + "Vertical"), Input.GetAxis(ControllerID + "Horizontal"));

                if (Input.GetButtonDown(ControllerID + "CameraAction"))
                    GetComponentInChildren<CameraController>().FlipCamera();
                if (Input.GetButtonUp(ControllerID + "CameraAction"))
                    GetComponentInChildren<CameraController>().FlipCamera();
                if ((Input.GetButtonDown(ControllerID + "Action")))
                {
                    if (CurrentPickup != null)
                    {
                        Pickup l_Pickup = CurrentPickup.GetComponent<Pickup>();
                        if (l_Pickup != null)
                            l_Pickup.UsePickup(gameObject);

                        if (l_Pickup.PickupUses <= 0)
                            l_Pickup.DeletePickup(gameObject);
                    }
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