using System;
using UnityEngine;

using Objects;

namespace Controllers
{
    public delegate void PlayerTriggerExit(PlayerTriggerExitEventArgs EventArgs);

    public class WayPointController : MonoBehaviour
    {
        public event PlayerTriggerExit OnPlayerTriggerExit;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnTriggerExit(Collider p_OtherCollider)
        {
            if (p_OtherCollider.name == "PlayerTriggerCollider")
            {
                GameObject l_OtherGameObject = p_OtherCollider.gameObject;

                /*
                 * Need a method to access the Player object for the Player from the PlayerTriggerCollider??                 * 
                 * 
                 * Store a Reference in the Kart Collider and then you just have to work your way up to there? 
                 * (Could always move the Collider up to the Top Level?)
                 *  OR
                 * Go to the Race Manager and Find the Player by the GameObject?
                 * 
                */

                if (OnPlayerTriggerExit != null)
                    OnPlayerTriggerExit(new PlayerTriggerExitEventArgs(this, /*l_Player,*/ DateTime.Now));
            }
        }
    }
}
