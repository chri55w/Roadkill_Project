using UnityEngine;
using System.Collections.Generic;

using Controllers;
using Objects;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        //public GameObject PlayerPrefab;
        public GameObject KartCameraPrefab;
        public List<GameObject> KartList;
        public List<Transform> StartPositions;
        public List<GameObject> PlayerList = new List<GameObject>();
        //Possible addition, code dictionary to be serializable thus removes need of above kart list   
        public Dictionary<string, GameObject> KartDictionary = new Dictionary<string, GameObject>();

        public TrackInfo SelectedTrackInfo;
        public Dictionary<string, List<WaypointController>> TrackedWaypoints = new Dictionary<string, List<WaypointController>>();
        public Dictionary<string, int> PlayerLapCount = new Dictionary<string, int>();

        private Dictionary<string, string> DebugParameters = new Dictionary<string, string>();

        void Awake()
        {
            //s_PlayerPrefab = PlayerPrefab;
            foreach (GameObject kart in KartList)
                KartDictionary.Add(kart.name, kart);

            foreach (WaypointController l_Waypoint in SelectedTrackInfo.Waypoints)
            {
                l_Waypoint.OnWaypointSetUnset += Waypoint_OnWaypointSetUnset;
            }

            foreach (GameObject l_GameObject in PlayerList)
            {
                Player l_Player = l_GameObject.GetComponent<Player>();

                if (l_Player != null)
                {
                    PlayerLapCount.Add(l_Player.Name, 0);
                    TrackedWaypoints.Add(l_Player.Name, new List<WaypointController>());
                }
            }
        }

        private void Waypoint_OnWaypointSetUnset(SetUnsetWaypointCollisionEventArgs e_EventArgs)
        {
            string l_PlayerName = e_EventArgs.e_Player.Name;
            int l_LastTrackedWaypointIndex = TrackedWaypoints[l_PlayerName].Count - 1;
            WaypointController l_LastTrackedWaypoint = null;

            try
            {
                l_LastTrackedWaypoint = TrackedWaypoints[l_PlayerName][l_LastTrackedWaypointIndex];
            }
            catch { }

            if (e_EventArgs.e_Waypoint == SelectedTrackInfo.StartFinishWaypoint)
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        PlayerLapCount[l_PlayerName] -= 1;

                        TrackedWaypoints[l_PlayerName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (PlayerLapCount[l_PlayerName] == 0 || SelectedTrackInfo.ConfirmLap(TrackedWaypoints[l_PlayerName]))
                    {
                        PlayerLapCount[l_PlayerName] += 1;
                    }

                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint == null || l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            TrackedWaypoints[l_PlayerName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }
            else
            {
                if (e_EventArgs.e_Type == SetUnsetWayPointType.Unset)
                {
                    if (e_EventArgs.e_Waypoint == l_LastTrackedWaypoint)
                    {
                        TrackedWaypoints[l_PlayerName].Remove(e_EventArgs.e_Waypoint);
                    }
                }
                else if (e_EventArgs.e_Type == SetUnsetWayPointType.Set)
                {
                    if (e_EventArgs.e_Waypoint != l_LastTrackedWaypoint)
                    {
                        if (l_LastTrackedWaypoint.NextWaypoints.Contains(e_EventArgs.e_Waypoint))
                            TrackedWaypoints[l_PlayerName].Add(e_EventArgs.e_Waypoint);
                    }
                }
            }

            // ****** Debug OnGui Information ******

            l_LastTrackedWaypointIndex = TrackedWaypoints[l_PlayerName].Count - 1;

            try
            {
                l_LastTrackedWaypoint = TrackedWaypoints[l_PlayerName][l_LastTrackedWaypointIndex];
            }
            catch { }

            string NextPossibleWaypoints = "";
            if (l_LastTrackedWaypoint != null)
            {
                foreach (WaypointController l_Waypoint in l_LastTrackedWaypoint.NextWaypoints)
                    NextPossibleWaypoints += l_Waypoint.ID.ToString() + ", ";

                UpdateDebugVariables(NextPossibleWaypoints, PlayerLapCount[l_PlayerName].ToString());
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
            l_NewCamera.GetComponent<CameraController>().PlayerFollowing = l_kart.gameObject;

            l_Player.GetComponent<Objects.Player>().MakePlayer(l_kart, p_ControllerID, StartPositions[PlayerList.Count], this);

            PlayerLapCount.Add(l_Player.name, 0);
            TrackedWaypoints.Add(l_Player.name, new List<WaypointController>());

            PlayerList.Add(l_Player);
        }

    }
}