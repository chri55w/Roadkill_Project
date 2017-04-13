using UnityEngine;
using System.Collections;

namespace Objects
{
    public class KillBox : MonoBehaviour
    {
        public BoxCollider Collider;
        public Transform RespawnLocation;

        public void Start()
        {
            Collider = GetComponent<BoxCollider>();
        }


        public void OnTriggerEnter(Collider p_OtherColldier)
        {
            GameObject l_Other = p_OtherColldier.transform.root.gameObject;

            if(l_Other.name.Contains("Driver"))
            {
                if (RespawnLocation != null)
                    l_Other.GetComponent<Driver>().ResetPosition(RespawnLocation.position, RespawnLocation.rotation);
                else
                    l_Other.GetComponent<Driver>().ResetPosition();
            }

        }
    }
}
