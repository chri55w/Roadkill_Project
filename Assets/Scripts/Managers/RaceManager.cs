using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        //public GameObject PlayerPrefab;
        public GameObject KartCameraPrefab;
        public List<GameObject> KartList;
        public List<Transform> StartPositions;

        //public static GameObject s_PlayerPrefab;
        public static List<GameObject> s_PlayerList = new List<GameObject>();
        private static GameObject s_KartCameraPrefab;
        public static Dictionary<string, GameObject> s_KartDictionary = new Dictionary<string, GameObject>();
        public static List<Transform> s_StartPositions;

        void Awake()
        {
            //s_PlayerPrefab = PlayerPrefab;
            s_KartCameraPrefab = KartCameraPrefab;
            s_StartPositions = StartPositions;
            foreach (GameObject kart in KartList)
                s_KartDictionary.Add(kart.name, kart);

        }

        public static void AddPlayer(string p_KartName, string p_ControllerID)
        {
            //if (s_PlayerPrefab == null)
            //    SetPlayerPrefab();

            GameObject l_Player = new GameObject();
            //Instantiate(s_PlayerPrefab) as GameObject;
            GameObject l_kart = Instantiate(s_KartDictionary[p_KartName]);
            l_kart.transform.SetParent(l_Player.transform);
            l_Player.name = "Player" + s_PlayerList.Count;
            l_Player.AddComponent<Objects.Player>();

            GameObject l_NewCamera = Instantiate(s_KartCameraPrefab);
            //l_NewCamera.AddComponent<Camera>(); 
            //Camera l_NewCamera = new Camera();
            //l_NewCamera.gameObject.AddComponent<Controllers.CameraController>();
            l_NewCamera.transform.SetParent(l_Player.transform);
            l_NewCamera.GetComponent<Controllers.CameraController>().PlayerFollowing = l_kart.gameObject;

            l_Player.GetComponent<Objects.Player>().MakePlayer(l_kart, p_ControllerID, s_StartPositions[s_PlayerList.Count]);
            s_PlayerList.Add(l_Player);
        }        


        //private static void SetPlayerPrefab()
        //{
        //    PlayerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Player");
            
        //}        
    }
}