using UnityEngine;
using System.Collections.Generic;

using Controllers;
using Objects;
using Events;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        private ObjectFadeController m_FadeController;
        private TrackInfo m_SelectedTrackInfo;
        public List<GameObject> m_Drivers = new List<GameObject>();
        private Dictionary<string, List<WaypointController>> m_DriverTrackedWaypoints = new Dictionary<string, List<WaypointController>>();
        private Dictionary<string, int> m_DriverLapCount = new Dictionary<string, int>();

        float RaceStartDelayTime = 8.0f;
        bool RaceStarted = false;


        public void Start()
        {
            for (int i = 0; i < m_Drivers.Count; i++)
            {
                if (m_Drivers[i].GetComponent<AIController>())
                    m_Drivers[i].GetComponent<AIController>().SetupTrack(m_SelectedTrackInfo.StartPositions[i], m_SelectedTrackInfo.StartingSpline);
                else if (m_Drivers[i].GetComponent<PlayerController>())
                    m_Drivers[i].GetComponent<PlayerController>().SetupTrack(m_SelectedTrackInfo.StartPositions[i], m_SelectedTrackInfo.StartingSpline);

            }
            SetCameras();
        }

        public void Update()
        {
            if (!RaceStarted)
            {
                RaceStartDelayTime -= Time.deltaTime;

                if (RaceStartDelayTime < 0.0f)
                {
                    StartRace();
                    RaceStarted = true;
                }
            }
        }

        private void StartRace()
        {
            for (int i = 0; i < m_Drivers.Count; i++)
            {
                if (m_Drivers[i].GetComponent<AIController>())
                    m_Drivers[i].GetComponent<AIController>().Active = true;
                else if (m_Drivers[i].GetComponent<PlayerController>())
                    m_Drivers[i].GetComponent<PlayerController>().Active = true;
            }
        }

        public void SetTrackInfo(TrackInfo p_TrackInfo)
        {
            m_SelectedTrackInfo = p_TrackInfo;

            foreach (WaypointController l_Waypoint in m_SelectedTrackInfo.Waypoints)
                l_Waypoint.OnWaypointSetUnset += Waypoint_OnWaypointSetUnset;
        }

        public void SetFadeController(ObjectFadeController p_FadeController)
        {
            m_FadeController = p_FadeController;
        }
        
        private void Waypoint_OnWaypointSetUnset(SetUnsetWaypointCollisionEventArgs e_EventArgs)
        {
            string l_DriverName = e_EventArgs.e_Driver.Name;
            int l_LastTrackedWaypointIndex = m_DriverTrackedWaypoints[l_DriverName].Count - 1;
            WaypointController l_LastTrackedWaypoint = null;
            
            try
            {
                l_LastTrackedWaypoint = m_DriverTrackedWaypoints[l_DriverName][l_LastTrackedWaypointIndex];
            }
            catch { }

            if (e_EventArgs.e_Waypoint == m_SelectedTrackInfo.StartFinishWaypoint)
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        m_DriverLapCount[l_DriverName] -= 1;

                        m_DriverTrackedWaypoints[l_DriverName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (m_DriverLapCount[l_DriverName] == 0 || m_SelectedTrackInfo.ConfirmLap(m_DriverTrackedWaypoints[l_DriverName]))
                    {
                        m_DriverLapCount[l_DriverName] += 1;
                    }

                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint == null || l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            m_DriverTrackedWaypoints[l_DriverName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }
            else
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        m_DriverTrackedWaypoints[l_DriverName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            m_DriverTrackedWaypoints[l_DriverName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }

            l_LastTrackedWaypointIndex = m_DriverTrackedWaypoints[l_DriverName].Count - 1;

            try
            {
                l_LastTrackedWaypoint = m_DriverTrackedWaypoints[l_DriverName][l_LastTrackedWaypointIndex];
            }
            catch { }
        }

        public void AddAI(AIController p_AIController, int p_KartMaterialIndex, int p_CharacterMaterialIndex)
        {
            GameObject l_AIDriver = new GameObject();
            GameObject l_Kart = Instantiate(p_AIController.Kart);
            GameObject l_Character = Instantiate(p_AIController.Character);
            GameObject l_InCarCharacter = Instantiate(p_AIController.InCarCharacter);
            l_Kart.transform.SetParent(l_AIDriver.transform);
            l_AIDriver.name = "Driver" + m_Drivers.Count;

            l_AIDriver.AddComponent<AIController>();
            l_AIDriver.GetComponent<AIController>().SetupDriver(l_AIDriver.name, l_Kart, l_Character, l_InCarCharacter, this, m_FadeController, p_KartMaterialIndex, p_CharacterMaterialIndex);
            l_AIDriver.GetComponent<AIController>().CenterPath = m_SelectedTrackInfo.StartingSpline;

            l_Kart.GetComponent<Rigidbody>().useGravity = true;
            
            m_DriverLapCount.Add(l_AIDriver.name, 0);
            m_DriverTrackedWaypoints.Add(l_AIDriver.name, new List<WaypointController>());

            m_Drivers.Add(l_AIDriver);

            //l_Character.SetActive(false);

            DontDestroyOnLoad(l_AIDriver);
            DontDestroyOnLoad(l_Character);            
        }

        public void AddPlayer(PlayerController p_PlayerController, GameObject p_CameraPrefab, int p_KartMaterialIndex, int p_CharacterMaterialIndex)
        {
            GameObject l_Player = new GameObject();
            GameObject l_Kart = Instantiate(p_PlayerController.Kart);
            GameObject l_Character = Instantiate(p_PlayerController.Character);
            GameObject l_InCarCharacter = Instantiate(p_PlayerController.InCarCharacter);
            l_Kart.transform.SetParent(l_Player.transform);
            l_Player.name = "Driver" + m_Drivers.Count;

            l_Player.AddComponent<PlayerController>();            
            l_Player.GetComponent<PlayerController>().SetupDriver(l_Player.name, l_Kart, l_Character, l_InCarCharacter, this, m_FadeController, p_KartMaterialIndex, p_CharacterMaterialIndex);
            l_Player.GetComponent<PlayerController>().ControllerID = p_PlayerController.ControllerID;

            l_Kart.GetComponent<Rigidbody>().useGravity = true;

            GameObject l_NewCamera = Instantiate(p_CameraPrefab);
            l_NewCamera.transform.SetParent(l_Player.transform);
            l_NewCamera.GetComponent<CameraController>().DriverFollowing = l_Kart.gameObject;
    
            m_DriverLapCount.Add(l_Player.name, 0);
            m_DriverTrackedWaypoints.Add(l_Player.name, new List<WaypointController>());

            m_Drivers.Add(l_Player);

            //l_Character.SetActive(false);

            DontDestroyOnLoad(l_Player);
            //DontDestroyOnLoad(l_Character);
        }

        private void SetCameras()
        {
            switch(m_Drivers.Count)
            {
                case 0:
                    break;
                case 1:
                    m_Drivers[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 1f));
                    break;
                case 2:
                    m_Drivers[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 1f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 0.5f));
                    break;
                case 3:
                    m_Drivers[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[2].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    break;
                case 4:
                    m_Drivers[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[2].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    m_Drivers[3].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0.5f, 0f, 0.5f, 0.5f));
                    break;
            }
        }

        //public WaypointController GetDriversNextWaypoint(string p_DriverName)
        //{
        //    int l_DriversLastTrackedWaypointIndex = m_DriverTrackedWaypoints[p_DriverName].Count - 1;

        //    if (l_DriversLastTrackedWaypointIndex < 0)
        //        return SelectedTrackInfo.StartFinishWaypoint;

        //    return m_DriverTrackedWaypoints[p_DriverName][l_DriversLastTrackedWaypointIndex].NextWaypoints[0];
        //}
    }
}