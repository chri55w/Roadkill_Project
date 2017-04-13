using UnityEngine;
using System.Collections.Generic;

using Controllers;
using Objects;
using Events;

using System.Linq;

namespace Managers
{
    public class RaceManager : MonoBehaviour
    {
        private MiniMapController m_MiniMapController;
        private ObjectFadeController m_FadeController;
        private TrackInfo m_SelectedTrackInfo;
        public List<GameObject> m_Drivers = new List<GameObject>();
        private Dictionary<string, List<WaypointController>> m_DriverTrackedWaypoints = new Dictionary<string, List<WaypointController>>();
        private Dictionary<string, int> m_DriverLapCount = new Dictionary<string, int>();
        private Canvas m_Canvas;
        private Dictionary<GameObject, float> m_DriverPositions = new Dictionary<GameObject, float>();
        private List<GameObject> m_DriverFinishingPositions = new List<GameObject>();
        public Sprite[] m_PositionTextures = new Sprite[12]; 
        public Sprite[] m_LapTextures = new Sprite[12];

        public GameObject CinematicCamera;
        private bool m_CinematicEnabled;

        float RaceStartDelay = 5.0f;
        public bool RaceStarted = false;
        float RaceEndDelay = 5.0f;
        public bool RaceFinishing = false;
        public bool RaceFinished = false;

        public int RaceLaps = 1;

        public void Awake()
        {
            CinematicCamera = Instantiate(Resources.Load<GameObject>("Cameras/CinematicCamera"));
            DontDestroyOnLoad(CinematicCamera);
        }

        public void Start()
        {
            for (int i = 0; i < m_Drivers.Count; i++)
            {
                if (m_Drivers[i].GetComponent<AIController>())
                    m_Drivers[i].GetComponent<AIController>().SetupTrack(m_SelectedTrackInfo.StartPositions[i], m_SelectedTrackInfo.StartingSpline, m_SelectedTrackInfo.LapSpline);
                else if (m_Drivers[i].GetComponent<PlayerController>())
                    m_Drivers[i].GetComponent<PlayerController>().SetupTrack(m_SelectedTrackInfo.StartPositions[i], m_SelectedTrackInfo.StartingSpline, m_SelectedTrackInfo.LapSpline);
            }

            SetHUDLayout();
            SetCameras();

            LoadPositionTextures();
            LoadLapTextures();
        }

        public void Update()
        {
            if (m_CinematicEnabled)
            {
                if (!RaceFinished && CinematicCamera.GetComponent<SimpleCinematicCamera>().IsFinished())
                {
                    m_CinematicEnabled = false;

                    SetCameras();
                }
                else if (RaceFinished)
                {
                    if (Input.GetButtonDown(m_Drivers[0].GetComponent<PlayerController>().ControllerID + "Action"))
                    {
                        DestroyScene();

                        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
                    }
                }
            }
            else if (!RaceStarted)
            {
                RaceStartDelay -= Time.deltaTime;

                if (RaceStartDelay < 0.0f)
                {
                    StartRace();
                    RaceStarted = true;
                }
            }
            else if (RaceFinishing)
            {
                RaceEndDelay -= Time.deltaTime;

                if (RaceEndDelay < 0.0f)
                {
                    EndRace();
                    RaceFinishing = false;
                    RaceFinished = true;
                }
            }
            
            UpdatePositions();
        }
        
        private void LoadPositionTextures()
        {
            m_PositionTextures[0] = Resources.Load<Sprite>("Gui/Positions/1st Position");
            m_PositionTextures[1] = Resources.Load<Sprite>("Gui/Positions/2nd Position");
            m_PositionTextures[2] = Resources.Load<Sprite>("Gui/Positions/3rd Position");
            m_PositionTextures[3] = Resources.Load<Sprite>("Gui/Positions/4th Position");
            m_PositionTextures[4] = Resources.Load<Sprite>("Gui/Positions/5th Position");
            m_PositionTextures[5] = Resources.Load<Sprite>("Gui/Positions/6th Position");
            m_PositionTextures[6] = Resources.Load<Sprite>("Gui/Positions/7th Position");
            m_PositionTextures[7] = Resources.Load<Sprite>("Gui/Positions/8th Position");
            m_PositionTextures[8] = Resources.Load<Sprite>("Gui/Positions/9th Position");
            m_PositionTextures[9] = Resources.Load<Sprite>("Gui/Positions/10th Position");
            m_PositionTextures[10] = Resources.Load<Sprite>("Gui/Positions/11th Position");
            m_PositionTextures[11] = Resources.Load<Sprite>("Gui/Positions/12th Position");
        }

        private void LoadLapTextures()
        {
            m_LapTextures[0] = Resources.Load<Sprite>("Gui/Laps/1st Lap");
            m_LapTextures[1] = Resources.Load<Sprite>("Gui/Laps/2nd Lap");
            m_LapTextures[2] = Resources.Load<Sprite>("Gui/Laps/3rd Lap");
            m_LapTextures[3] = Resources.Load<Sprite>("Gui/Laps/4th Lap");
            m_LapTextures[4] = Resources.Load<Sprite>("Gui/Laps/5th Lap");
            m_LapTextures[5] = Resources.Load<Sprite>("Gui/Laps/6th Lap");
            m_LapTextures[6] = Resources.Load<Sprite>("Gui/Laps/7th Lap");
            m_LapTextures[7] = Resources.Load<Sprite>("Gui/Laps/8th Lap");
            m_LapTextures[8] = Resources.Load<Sprite>("Gui/Laps/9th Lap");
            m_LapTextures[9] = Resources.Load<Sprite>("Gui/Laps/10th Lap");
            m_LapTextures[10] = Resources.Load<Sprite>("Gui/Laps/11th Lap");
            m_LapTextures[11] = Resources.Load<Sprite>("Gui/Laps/12th Lap");
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
        
        public void SetHUDLayout()
        {
            m_MiniMapController.SetScreenPosition(RectTransform.Edge.Top, RectTransform.Edge.Right, new Vector2(10, 10), new Vector2(250, 150));

            switch (m_Drivers.Count)
            {
                case 0:
                    break;
                case 1:
                    m_MiniMapController.SetScreenPosition(RectTransform.Edge.Top, RectTransform.Edge.Right, new Vector2(10, 10), new Vector2(250, 150));
                    break;
                case 2:
                    m_MiniMapController.SetScreenPosition(RectTransform.Edge.Top, RectTransform.Edge.Right, new Vector2(-135, -85), new Vector2(250, 150));
                    break;
            }
        }
        
        public void UpdatePositions()
        {
            m_DriverPositions.Clear();

            foreach (string l_DriverName in m_DriverLapCount.Keys)
            {
                foreach (GameObject l_Driver in m_Drivers)
                {
                    if (l_Driver.name.Equals(l_DriverName))
                    {
                        if (m_DriverLapCount[l_DriverName] > 0)                        
                            m_DriverPositions.Add(l_Driver, l_Driver.GetComponent<Driver>().GetPosition() + (m_DriverLapCount[l_DriverName] + 1));
                        
                        else
                            m_DriverPositions.Add(l_Driver, l_Driver.GetComponent<Driver>().GetPosition());
                    }

                }
            }

            // Use stored list from above loop
            foreach (GameObject l_Driver in m_DriverPositions.Keys)
            {
                // Position Icons are stored 1 - 12
                // Therefore to track positions need to begin from last (m_Drivers.Count)
                // And decrement for each Driver players are ahead
                int l_Position = m_Drivers.Count;
                
                foreach(float l_DriverPosition in m_DriverPositions.Values)
                {                    
                    if(m_DriverPositions[l_Driver] > l_DriverPosition)
                    {
                        l_Position--;
                    }
                }

                PlayerController l_Player = l_Driver.GetComponent<PlayerController>();

                if (l_Player != null)
                {
                    l_Player.SetPositionSprite(m_PositionTextures[l_Position - 1]);

                    int l_LapCountIndex = m_DriverLapCount[l_Player.name] > 0 ? (m_DriverLapCount[l_Player.name] - 1) : 0;

                    l_Player.SetLapSprite(m_LapTextures[l_LapCountIndex]);
                }
            }
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
                        //Check if Driver has finished the Race
                        if (m_DriverLapCount[l_DriverName] == RaceLaps)
                            FinishRaceForDriver(e_EventArgs.e_Driver);
                        else 
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

        private void FinishRaceForDriver(Driver p_Driver)
        {
            Debug.Log(p_Driver.name + " Finished");

            m_DriverFinishingPositions.Add(p_Driver.gameObject);

            p_Driver.IsFinished = true;

            if (p_Driver.GetType() == typeof(PlayerController))
            {
                PlayerController l_PlayerController = p_Driver as PlayerController;

                if (AllPlayersHaveFinished())
                {
                    List<GameObject> l_NonFinishedDrivers = m_Drivers.Where(d => !d.GetComponent<Driver>().IsFinished).ToList();

                    List<GameObject> l_SortedNonFinishedDrivers = l_NonFinishedDrivers.OrderByDescending(d => (d.GetComponent<Driver>().GetPosition() + (m_DriverLapCount[d.GetComponent<Driver>().Name] + 1))).ToList();

                    foreach (GameObject l_NonFinishedDriver in l_SortedNonFinishedDrivers)
                    {
                        m_DriverFinishingPositions.Add(l_NonFinishedDriver);
                    }

                    RaceFinishing = true;
                    RaceEndDelay = 5.0f;
                }
            }
            else if (p_Driver.GetType() == typeof(AIController))
            {
                AIController l_AIController = p_Driver as AIController;
            }
        }

        private void EndRace()
        {
            CinematicCamera.GetComponent<SimpleCinematicCamera>().MovementSpline = m_SelectedTrackInfo.PodiumCinematicSpline;
            CinematicCamera.GetComponent<SimpleCinematicCamera>().CameraTarget = m_SelectedTrackInfo.PodiumCinematicTarget;
            CinematicCamera.GetComponent<SimpleCinematicCamera>().ResetTime();
            
            for (int i = 0; i < 3; i++)
            {
                GameObject l_DriverObject = m_DriverFinishingPositions[i];

                Driver l_Driver = l_DriverObject.GetComponent<Driver>();

                l_Driver.Kart.transform.position = m_SelectedTrackInfo.PodiumKartPositions[i].transform.position;
                l_Driver.Kart.transform.rotation = m_SelectedTrackInfo.PodiumKartPositions[i].transform.rotation;

                l_Driver.Character.transform.position = m_SelectedTrackInfo.PodiumCharacterPositions[i].transform.position;
                l_Driver.Character.transform.rotation = m_SelectedTrackInfo.PodiumCharacterPositions[i].transform.rotation;

                l_Driver.Kart.GetComponent<KartController>().UpdateBloodSpray(3);

                l_Driver.Character.GetComponent<Character>().SetCharacterPodiumAnimation(i+1);
                m_SelectedTrackInfo.PodiumAssets.SetActive(true);

                l_Driver.Kart.GetComponent<Rigidbody>().isKinematic = true;
            }
                        
            m_CinematicEnabled = true;

            SetCameras();
        }

        private void DestroyScene()
        {
            Destroy(m_FadeController);
            Destroy(CinematicCamera);
            Destroy(m_Canvas.gameObject);
            foreach (GameObject l_GameObject in m_Drivers)
                Destroy(l_GameObject);
            Destroy(m_SelectedTrackInfo.gameObject);
            foreach (GameObject l_GameObject in GameObject.FindObjectsOfType(typeof(GameObject)))
                Destroy(l_GameObject);

            Destroy(this.gameObject);
        }

        private bool AllPlayersHaveFinished()
        {
            List<GameObject> l_NonFinishedDrivers = m_Drivers.Where(d => !d.GetComponent<Driver>().IsFinished).ToList();

            foreach (GameObject l_GameObject in l_NonFinishedDrivers)
            {
                Driver l_Driver = l_GameObject.GetComponent<Driver>();

                if (l_Driver.GetType() == typeof(PlayerController))
                    return false;
            }

            return true;
        }

        public void SetupCanvas()
        {
            GameObject l_CanvasObject = new GameObject();
            m_Canvas = l_CanvasObject.AddComponent<Canvas>();
            l_CanvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            m_Canvas.name = "Canvas";

            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            DontDestroyOnLoad(l_CanvasObject);
        }

        public void ConfigureMinimap()
        {
            GameObject l_MiniMapControllerObject = new GameObject();
            m_MiniMapController = l_MiniMapControllerObject.AddComponent<MiniMapController>();
            l_MiniMapControllerObject.name = "MiniMap Object";

            l_MiniMapControllerObject.transform.SetParent(m_Canvas.gameObject.transform);

            m_MiniMapController.AddComponents();

            m_MiniMapController.SetCharacterIconSize(new Vector2(24, 24));
            m_MiniMapController.SetMapSize(m_SelectedTrackInfo.MapSize);
            m_MiniMapController.SetCharacterOffset(m_SelectedTrackInfo.MiniMapCharacterOffset);
            m_MiniMapController.SetTrackImage(m_SelectedTrackInfo.MiniMapMaterial);

            DontDestroyOnLoad(m_Canvas);
        }

        public void AddAI(GameObject p_AIDriver)
        {
            AddDriver(p_AIDriver);
            
            p_AIDriver.GetComponent<AIController>().CenterPath = m_SelectedTrackInfo.StartingSpline;
        }

        public void AddPlayer(GameObject p_PlayerDriver, GameObject p_CameraPrefab, GameObject p_HUDPrefab)
        {
            Driver l_Driver = p_PlayerDriver.GetComponent<Driver>();

            AddDriver(p_PlayerDriver);
            
            GameObject l_NewCamera = Instantiate(p_CameraPrefab);
            l_NewCamera.transform.SetParent(p_PlayerDriver.transform);
            l_NewCamera.GetComponent<CameraController>().DriverFollowing = l_Driver.Kart.gameObject;

            GameObject l_CanvasObject = Instantiate(p_HUDPrefab);
            l_CanvasObject.name = l_Driver.name + " Canvas";

            PlayerController l_PlayerController = p_PlayerDriver.GetComponent<PlayerController>();
            l_PlayerController.KartCamera = l_NewCamera.GetComponent<Camera>();
            l_PlayerController.SetupCanvas(l_CanvasObject.GetComponent<HUDController>());
            l_CanvasObject.transform.SetParent(p_PlayerDriver.transform);
            
            SetCameras();
        }

        private void AddDriver(GameObject p_DriverObject)
        {
            p_DriverObject.name = "Driver" + m_Drivers.Count;

            Driver l_Driver = p_DriverObject.GetComponent<Driver>();
            
            l_Driver.SetupDriver(p_DriverObject.name, l_Driver.Kart, l_Driver.Character, l_Driver.CharacterIcon, this, m_FadeController);

            m_DriverLapCount.Add(p_DriverObject.name, 0);
            m_DriverTrackedWaypoints.Add(p_DriverObject.name, new List<WaypointController>());

            l_Driver.Kart.GetComponent<Rigidbody>().useGravity = true;

            m_Drivers.Add(p_DriverObject);

            m_MiniMapController.AddDriver(p_DriverObject.GetComponent<Driver>());
            
            DontDestroyOnLoad(p_DriverObject);
        }

        private void DisableDriverCameras()
        {
            foreach (GameObject l_Driver in m_Drivers)
            {
                PlayerController l_Player = l_Driver.GetComponent<PlayerController>();

                if (l_Player != null)
                    l_Player.GetComponentInChildren<Camera>().enabled = false;
            }
        }

        private void EnableDriverCameras()
        {
            foreach (GameObject l_Driver in m_Drivers)
            {
                PlayerController l_Player = l_Driver.GetComponent<PlayerController>();

                if (l_Player != null)
                    l_Player.GetComponentInChildren<Camera>().enabled = true;
            }
        }

        private void SetCameras()
        {
            if (m_CinematicEnabled)
            {
                CinematicCamera.GetComponent<CameraTools>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 1f));

                DisableDriverCameras();

                CinematicCamera.GetComponent<Camera>().enabled = true;

                return;
            }
            else
            {
                CinematicCamera.GetComponent<Camera>().enabled = false;

                EnableDriverCameras();
            }

            int l_HumanPlayers = 0;
            foreach(GameObject l_Driver in m_Drivers)
            {
                PlayerController l_Player = l_Driver.GetComponent<PlayerController>();

                if (l_Player != null)
                    l_HumanPlayers++;
            }

            switch(l_HumanPlayers)
            {
                case 0:
                    break;
                case 1:
                    m_Drivers[0].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 1f));
                    break;
                case 2:
                    m_Drivers[0].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0.5f, 1f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0f, 1f, 0.5f));
                    break;
                case 3:
                    m_Drivers[0].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[2].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    break;
                case 4:
                    m_Drivers[0].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[1].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    m_Drivers[2].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, 0f, 0.5f, 0.5f));
                    m_Drivers[3].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0.5f, 0f, 0.5f, 0.5f));
                    break;
            }
        }

        public void BeginOpeningCinematic()
        {
            m_CinematicEnabled = true;

            CinematicCamera.GetComponent<SimpleCinematicCamera>().MovementSpline = m_SelectedTrackInfo.OpeningCinematicSpline;
            CinematicCamera.GetComponent<SimpleCinematicCamera>().CameraTarget = m_SelectedTrackInfo.OpeningCinematicTarget;
        }
    }
}