using UnityEngine;
using System.Collections;

using Objects;
using Managers;

namespace Controllers
{
    public class PlayerController : Driver
    {
        public string ControllerID;
        public Camera KartCamera;

        public float MaxExpectedSpeed = 36.0f;

        private HUDController m_HUD;
        private Sprite m_NoPickupIcon;

        new void Start()
        {
            base.Start();

            m_NoPickupIcon = Resources.Load<Sprite>("GUI/No Weapon");
        }

        void FixedUpdate()
        {

        }

        void Update()
        {
            RefreshHUD();

            if (!Active || IsFinished)
                return;

            if (DeathCheck() == false)
            {
                float l_HorizontalInput = Input.GetAxis(ControllerID + "Horizontal");
                float l_VerticalInput = Input.GetAxis(ControllerID + "Vertical");

                Kart.GetComponent<KartController>().Move(l_VerticalInput, l_HorizontalInput);

                Character.GetComponent<Character>().UpdateHorizontalInput(l_HorizontalInput);

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
                if (Input.GetButtonDown(ControllerID + "MenuCycleButtonTwo"))
                {
                    ResetPosition();
                }
            }
        }

        private void RefreshHUD()
        {
            if (m_HUD != null)
            {
                if (CurrentPickup != null)
                    m_HUD.UpdatePickupIcon(CurrentPickup.GetComponent<Pickup>().GetCurrentIcon());
                else
                    m_HUD.UpdatePickupIcon(m_NoPickupIcon);

                float l_SpeedPercentage = Kart.GetComponent<Rigidbody>().velocity.magnitude / MaxExpectedSpeed;

                m_HUD.UpdateSpeedometer(l_SpeedPercentage);
            }
        }

        public void SetupCanvas(HUDController p_HUDController)
        {
            m_HUD = p_HUDController;

            Canvas l_Canvas = m_HUD.GetComponent<Canvas>();
            l_Canvas.worldCamera = KartCamera;
            l_Canvas.planeDistance = 1.0f;

            m_HUD.CharacterIcon.sprite = CharacterIcon;
        }

        public void SetPositionSprite(Sprite p_PositionSprite)
        {
            m_HUD.UpdatePlayerPosition(p_PositionSprite);
        }

        public void SetLapSprite(Sprite p_LapSprite)
        {
            m_HUD.UpdateLapNumber(p_LapSprite);
        }

    }
}