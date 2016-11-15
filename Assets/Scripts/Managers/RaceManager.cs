using UnityEngine;
using System.Collections.Generic;

using Controllers;
using Objects;
using Events;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        public Camera MapCamera;
        public GameObject KartCameraPrefab;
        public List<GameObject> KartList;
        public List<Transform> StartPositions;

        public List<GameObject> DriverList = new List<GameObject>();
        //Possible addition, code dictionary to be serializable thus removes need of above kart list
        public Dictionary<string, GameObject> KartDictionary = new Dictionary<string, GameObject>();

        public TrackInfo SelectedTrackInfo;
        public Dictionary<string, List<WaypointController>> TrackedWaypoints = new Dictionary<string, List<WaypointController>>();
        public Dictionary<string, int> DriverLapCount = new Dictionary<string, int>();

        private Dictionary<string, string> DebugParameters = new Dictionary<string, string>();

        void Awake()
        {
            foreach (GameObject kart in KartList)
                KartDictionary.Add(kart.name, kart);

            SetCameras();

            foreach (WaypointController l_Waypoint in SelectedTrackInfo.Waypoints)
            {
                l_Waypoint.OnWaypointSetUnset += Waypoint_OnWaypointSetUnset;
            }

            foreach (GameObject l_GameObject in DriverList)
            {
                Driver l_Driver = l_GameObject.GetComponent<Driver>();

                if (l_Driver != null)
                {
                    DriverLapCount.Add(l_Driver.Name, 0);
                    TrackedWaypoints.Add(l_Driver.Name, new List<WaypointController>());
                }
            }
        }

        void Update()
        {
            //first check if any Drivers exist
            if (DriverList.Count >= 0 && DriverList.Count <= 4)
            {
                //Check for Drivers pressing start
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
                        AddPlayer("Fox Kart 1", "JS2");
                        SetCameras();
                    }
                }
                else if (Input.GetButton("JS3Start"))
                {
                    if (DoesControllerIDExist("JS3") == false)
                    {
                        AddPlayer("Fox Kart 1", "JS3");
                        SetCameras();
                    }
                }
                else if (Input.GetButton("JS4Start"))
                {
                    if (DoesControllerIDExist("JS4") == false)
                    {
                        AddPlayer("Fox Kart 1", "JS4");
                        SetCameras();
                    }
                }
            }

            //Optimize, only needs to be called when a new player is added
            
        }

        private void Waypoint_OnWaypointSetUnset(SetUnsetWaypointCollisionEventArgs e_EventArgs)
        {
            string l_DriverName = e_EventArgs.e_Driver.Name;
            int l_LastTrackedWaypointIndex = TrackedWaypoints[l_DriverName].Count - 1;
            WaypointController l_LastTrackedWaypoint = null;
            
            try
            {
                l_LastTrackedWaypoint = TrackedWaypoints[l_DriverName][l_LastTrackedWaypointIndex];
            }
            catch { }

            if (e_EventArgs.e_Waypoint == SelectedTrackInfo.StartFinishWaypoint)
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        DriverLapCount[l_DriverName] -= 1;

                        TrackedWaypoints[l_DriverName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (DriverLapCount[l_DriverName] == 0 || SelectedTrackInfo.ConfirmLap(TrackedWaypoints[l_DriverName]))
                    {
                        DriverLapCount[l_DriverName] += 1;
                    }

                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint == null || l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            TrackedWaypoints[l_DriverName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }
            else
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        TrackedWaypoints[l_DriverName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            TrackedWaypoints[l_DriverName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }

            // ****** Debug OnGui Information ******

            l_LastTrackedWaypointIndex = TrackedWaypoints[l_DriverName].Count - 1;

            try
            {
                l_LastTrackedWaypoint = TrackedWaypoints[l_DriverName][l_LastTrackedWaypointIndex];
            }
            catch { }

            string NextPossibleWaypoints = "";
            if (l_LastTrackedWaypoint != null)
            {
                foreach (WaypointController l_Waypoint in l_LastTrackedWaypoint.NextWaypoints)
                    NextPossibleWaypoints += l_Waypoint.ID.ToString() + ", ";

                UpdateDebugVariables(NextPossibleWaypoints, DriverLapCount[l_DriverName].ToString());
            }
        }

        private void UpdateDebugVariables(string p_NextWaypoints, string p_LapNumber)
        {
            DebugParameters["Next Waypoints"] = p_NextWaypoints;
            DebugParameters["Lap Number:"] = p_LapNumber;
        }

        public void OnGUI()
        {
            GUI.backgroundColor = Color.black;
            int l_YOffset = -25;

            foreach (KeyValuePair<string, string> DebugParameter in DebugParameters)
            {
                int ScreenBottom = Screen.height;

                GUI.TextField(new Rect(10, ScreenBottom + l_YOffset, 170, 20), string.Format("{0}: {1}", DebugParameter.Key, DebugParameter.Value));

                l_YOffset -= 25;
            }
            if (GUI.Button(new Rect(Screen.width - 150, Screen.height - 25, 125, 20), "Spawn AI Driver"))
                AddAI("Beaver Kart 1");
        }

        public void AddAI(string p_KartName)
        {
            GameObject l_AIDriver = new GameObject();
            GameObject l_kart = Instantiate(KartDictionary[p_KartName]);
            l_kart.transform.SetParent(l_AIDriver.transform);
            l_AIDriver.name = "Driver" + DriverList.Count;
            l_AIDriver.AddComponent<AIController>();
            
            l_AIDriver.GetComponent<AIController>().MakeDriver(l_kart, StartPositions[DriverList.Count], this);
            
            DriverLapCount.Add(l_AIDriver.name, 0);
            TrackedWaypoints.Add(l_AIDriver.name, new List<WaypointController>());

            DriverList.Add(l_AIDriver);
        }

        public void AddPlayer(string p_KartName, string p_ControllerID)
        {
            GameObject l_Player = new GameObject();
            GameObject l_kart = Instantiate(KartDictionary[p_KartName]);
            l_kart.transform.SetParent(l_Player.transform);
            l_Player.name = "Driver" + DriverList.Count;
            l_Player.AddComponent<PlayerController>();

            GameObject l_NewCamera = Instantiate(KartCameraPrefab);
            l_NewCamera.transform.SetParent(l_Player.transform);
            l_NewCamera.GetComponent<CameraController>().DriverFollowing = l_kart.gameObject;

            l_Player.GetComponent<PlayerController>().MakeDriver(l_kart, StartPositions[DriverList.Count], this);

            l_Player.GetComponent<PlayerController>().ControllerID = p_ControllerID;
    
            DriverLapCount.Add(l_Player.name, 0);
            TrackedWaypoints.Add(l_Player.name, new List<WaypointController>());

            DriverList.Add(l_Player);   
        }
        
        private bool DoesControllerIDExist(string p_controllerID)
        {
            bool l_IsControllerIDTaken = false;
            foreach(GameObject l_Driver in DriverList)
            {
                if (l_Driver.GetComponent<PlayerController>() != null)
                {
                    if (l_Driver.GetComponent<PlayerController>().ControllerID.Trim().Equals(p_controllerID))
                    {
                        l_IsControllerIDTaken = true;
                        return l_IsControllerIDTaken;
                    }
                }
                else
                    l_IsControllerIDTaken = false;
            }

            return l_IsControllerIDTaken;
        }

        private void SetCameras()
        {
            switch(DriverList.Count)
            {
                case 0:
                    MapCamera.enabled = true;
                    MapCamera.rect = new Rect(0f, 0f, 1f, 1f);
                    break;
                case 1:
                    MapCamera.enabled = false;
                    DriverList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 1f));
                    break;
                case 2:
                    MapCamera.enabled = false;
                    DriverList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 1f, 0.5f));
                    DriverList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 0.5f));
                    break;
                case 3:
                    MapCamera.enabled = true;
                    MapCamera.rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    DriverList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    DriverList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    DriverList[2].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    break;
                case 4:
                    MapCamera.enabled = false;                    
                    DriverList[0].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    DriverList[1].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    DriverList[2].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    DriverList[3].GetComponentInChildren<Controllers.CameraController>().SetCameraScreenSize(new Rect(0.5f, 0f, 0.5f, 0.5f));
                    break;
            }
        }

        public WaypointController GetDriversNextWaypoint(string p_DriverName)
        {
            int l_DriversLastTrackedWaypointIndex = TrackedWaypoints[p_DriverName].Count - 1;

            if (l_DriversLastTrackedWaypointIndex < 0)
                return SelectedTrackInfo.StartFinishWaypoint;

            return TrackedWaypoints[p_DriverName][l_DriversLastTrackedWaypointIndex].NextWaypoints[0];
        }
    }
}