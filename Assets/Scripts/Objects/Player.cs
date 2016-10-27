using UnityEngine;
using System.Collections;

namespace Objects
{
    [System.Serializable]
    public class Player : MonoBehaviour
    {
        //public Transform StartPosition;
        //public Camera playerCamera;
        public string controllerID;
        public GameObject Kart;
        
        public void MakePlayer(GameObject p_Kart, string p_ControllerID, Transform p_StartPosition)
        {
            Kart = p_Kart;
            controllerID = p_ControllerID;
            
            transform.position = p_StartPosition.position;
            transform.rotation = p_StartPosition.rotation;
        }

        // Use this for initialization
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
