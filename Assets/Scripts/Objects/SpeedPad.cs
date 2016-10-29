using UnityEngine;
using System.Collections;

namespace Objects
{
    public class SpeedPad : MonoBehaviour
    {
        public float BoostForce;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerStay(Collider p_OtherCollider)
        {
            if(p_OtherCollider.transform.root.name.Contains("Player"))
            {
               p_OtherCollider.transform.root.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce);               
            }
        }
    }
}
