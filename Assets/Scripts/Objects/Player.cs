using UnityEngine;
using System.Collections;

using Managers;

namespace Objects
{
    [System.Serializable]
    public class Player : MonoBehaviour
    {
        //public Transform StartPosition;
        //public Camera playerCamera;
        public string controllerID;
        public GameObject Kart;

        private RaceManager m_RaceManager;
        public string Name;
        
        public void MakePlayer(GameObject p_Kart, string p_ControllerID, Transform p_StartPosition, RaceManager p_RaceManager)
        {
            Kart = p_Kart;
            controllerID = p_ControllerID;
            
            transform.position = p_StartPosition.position;
            transform.rotation = p_StartPosition.rotation;

            m_RaceManager = p_RaceManager;
        }

        // Use this for initialization
        void Start()
        {
            Name = transform.root.gameObject.name;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            Kart.GetComponent<Controllers.KartController>().Move(Input.GetAxis(controllerID + "Vertical"), Input.GetAxis(controllerID + "Horizontal"));
            if (Input.GetButtonDown(controllerID + "FlipCamera"))
                GetComponentInChildren<Controllers.CameraController>().FlipCamera();
            if(Input.GetButtonUp(controllerID + "FlipCamera"))
                GetComponentInChildren<Controllers.CameraController>().FlipCamera();
        }
    }
}
