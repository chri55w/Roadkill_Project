using UnityEngine;
using System.Collections;

using Objects;
using Managers;

namespace Controllers
{
    public class PlayerController : Driver
    {
        public string ControllerID;
        public Texture2D NoPickupIcon;
        public Texture2D Position;
        
        new void Start()
        {
            base.Start();
            NoPickupIcon = Resources.Load<Texture2D>("GUI/No Weapon");
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
            if (m_RaceManager != null && m_RaceManager.RaceStarted)
            {
                if (CurrentPickup != null)
                {
                    GUI.DrawTexture(new Rect(Screen.width - 60, 10, 50, 50), CurrentPickup.GetComponent<Pickup>().GetCurrentIcon());
                }
                else
                {
                    GUI.DrawTexture(new Rect(Screen.width - 60, 10, 50, 50), NoPickupIcon);
                }

                GUI.DrawTexture(new Rect(60, 10, 50, 50), Position);
            }
        }

    }
}