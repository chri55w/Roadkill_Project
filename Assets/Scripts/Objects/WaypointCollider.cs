using System;
using UnityEngine;
using System.Collections;


namespace Objects
{
    public delegate void PlayerTriggerExit(PlayerTriggerExitEventArgs EventArgs);

    public class WaypointCollider : MonoBehaviour
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
                Player l_Player = p_OtherCollider.transform.root.gameObject.GetComponent<Player>();

                if (l_Player != null)
                    if (OnPlayerTriggerExit != null)
                        OnPlayerTriggerExit(new PlayerTriggerExitEventArgs(l_Player, DateTime.Now));
            }
        }
    }
}
