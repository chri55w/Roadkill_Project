using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        public Camera MapCamera;
        public GameObject KartCameraPrefab;
        public List<GameObject> KartList;
        public List<Transform> StartPositions;
        public List<GameObject> PlayerList = new List<GameObject>();     
        //Possible addition, code dictionary to be serializable thus removes need of above kart list   
        public Dictionary<string, GameObject> KartDictionary = new Dictionary<string, GameObject>();             

        void Awake()
        {
            //s_PlayerPrefab = PlayerPrefab;
            foreach (GameObject kart in KartList)
                KartDictionary.Add(kart.name, kart);
            SetCameras();
        }

        void Update()
        {
            //first check if any players exist
            if (PlayerList.Count >= 0 && PlayerList.Count <= 4)
            {
                //Check for players pressing start
                if (Input.GetButton("JS1Start"))
                {
                    if (DoesControllerIDExist("JS1") == false)
                    {
                        AddPlayer("Beaver Kart 1", "JS1");
                        SetCameras();
                    }
                }
                else if (Input.GetButton("JS2Start"))
                {
                    if (DoesControllerIDExist("JS2") == false)
                    { 
                        AddPlayer("Beaver Kart 1", "JS2");
                        SetCameras();
                    }
                }
                else if (Input.GetButton("JS3Start"))
                {
                    if (DoesControllerIDExist("JS3") == false)
                    {
                        AddPlayer("Beaver Kart 1", "JS3");
                        SetCameras();
                    }
                }
                else if (Input.GetButton("JS4Start"))
                {
                    if (DoesControllerIDExist("JS4") == false)
                    {
                        AddPlayer("Beaver Kart 1", "JS4");
                        SetCameras();
                    }
                }
            }

            //Optimize, only needs to be called when a new player is added
            
        }

        public void AddPlayer(string p_KartName, string p_ControllerID)
        {
            GameObject l_Player = new GameObject();
            //Instantiate(s_PlayerPrefab) as GameObject;
            GameObject l_kart = Instantiate(KartDictionary[p_KartName]);
            l_kart.transform.SetParent(l_Player.transform);
            l_Player.name = "Player" + PlayerList.Count;
            l_Player.AddComponent<Objects.Player>();

            GameObject l_NewCamera = Instantiate(KartCameraPrefab);
            l_NewCamera.transform.SetParent(l_Player.transform);
            l_NewCamera.GetComponent<Controllers.CameraController>().PlayerFollowing = l_kart.gameObject;

            l_Player.GetComponent<Objects.Player>().MakePlayer(l_kart, p_ControllerID, StartPositions[PlayerList.Count], this);
            PlayerList.Add(l_Player);
            Debug.Log(p_ControllerID);     
        }
        
        private bool DoesControllerIDExist(string p_controllerID)
        {
            bool l_IsControllerIDTaken = false;
            foreach(GameObject l_player in PlayerList)
            {
                if (l_player.GetComponent<Objects.Player>().controllerID.Trim().Equals(p_controllerID))
                {
                    l_IsControllerIDTaken = true;
                    return l_IsControllerIDTaken;
                }
                else
                    l_IsControllerIDTaken = false;
            }

            return l_IsControllerIDTaken;
        }

        private void SetCameras()
        {
            switch(PlayerList.Count)
            {
                case 0:
                    MapCamera.enabled = true;
                    MapCamera.rect = new Rect(0f, 0f, 1f, 1f);
                    break;
                case 1:
                    MapCamera.enabled = false;
                    PlayerList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 1f));
                    break;
                case 2:
                    MapCamera.enabled = false;
                    PlayerList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 1f, 0.5f));
                    PlayerList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 0.5f));
                    break;
                case 3:
                    MapCamera.enabled = true;
                    MapCamera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    PlayerList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    PlayerList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    PlayerList[2].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    break;
                case 4:
                    MapCamera.enabled = false;                    
                    PlayerList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    PlayerList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    PlayerList[2].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    PlayerList[3].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0f, 0.5f, 0.5f));
                    break;
            }
        }
    }
}