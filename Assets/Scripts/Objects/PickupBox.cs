using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Objects
{
    public class PickupBox : MonoBehaviour
    {
        //For when multiple pickups exist
        //TODO: Organise Pickup list in relation to position dependancy 
        //public List<GameObject> AvailablePickups = new List<GameObject>();

        public GameObject Pickup;
        public float CooldownTimerLimit;

        private float m_CooldownTimer = 0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (m_CooldownTimer > 0)
                m_CooldownTimer -= Time.deltaTime;

            gameObject.GetComponent<BoxCollider>().enabled = m_CooldownTimer > 0 ? false : true;
            gameObject.GetComponent<MeshRenderer>().enabled = m_CooldownTimer > 0 ? false : true;
        }

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            GameObject l_OtherGameObject = p_OtherCollider.transform.root.gameObject;
            if (l_OtherGameObject.name.Contains("Player"))
            {
                if (l_OtherGameObject.GetComponent<Player>().CurrentPickup == null)
                {
                    //TODO: Set random range based on player position
                    //This will be for when multipickups exist
                    //GameObject l_Pickup = Instantiate(AvailablePickups[(int)Random.Range(0, AvailablePickups.Count)]);
                    GameObject l_Pickup = Instantiate(Pickup);
                    l_OtherGameObject.GetComponent<Player>().CurrentPickup = l_Pickup;

                    m_CooldownTimer = CooldownTimerLimit;
                }
                else if(l_OtherGameObject.GetComponent<Player>().CurrentPickup.GetComponent<Pickup>().PickupLevel < 3)
                {
                    l_OtherGameObject.GetComponent<Player>().CurrentPickup.GetComponent<Pickup>().PickupLevel += 1;
                }
            }
        }
    }
}
