using UnityEngine;
using System.Collections;

using Managers;
using Controllers;

namespace Objects
{
    public abstract class Driver : MonoBehaviour
    {
        public GameObject Kart;
        public GameObject CurrentPickup;
        public string Name;

        protected RaceManager m_RaceManager;

        public void MakeDriver(GameObject p_Kart, Transform p_StartPosition, RaceManager p_RaceManager)
        {
            Kart = p_Kart;

            transform.position = p_StartPosition.position;
            transform.rotation = p_StartPosition.rotation;

            m_RaceManager = p_RaceManager;
        }


        public void Start()
        {
            Name = transform.root.gameObject.name;
        }
    }
}