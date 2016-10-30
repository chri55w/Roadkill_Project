using System;
using UnityEngine;
using System.Collections;

using Events;

namespace Objects
{
    public delegate void DriverTriggerExit(DriverTriggerExitEventArgs EventArgs);

    public class WaypointCollider : MonoBehaviour
    {
        public event DriverTriggerExit OnDriverTriggerExit;

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
            if (p_OtherCollider.name == "DriverTriggerCollider")
            {
                Driver l_Driver = p_OtherCollider.transform.root.gameObject.GetComponent<Driver>();

                if (l_Driver != null)
                    if (OnDriverTriggerExit != null)
                        OnDriverTriggerExit(new DriverTriggerExitEventArgs(l_Driver, DateTime.Now));
            }
        }
    }
}
