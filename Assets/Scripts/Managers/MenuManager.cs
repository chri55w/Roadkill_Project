using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Managers;
using Controllers;
using Objects;

public class MenuManager : MonoBehaviour
{
    public List<MenuPanel> MenuPanels = new List<MenuPanel>();
    public Dropdown ScreenResolutionsDropdown;
    public UnityEngine.EventSystems.EventSystem UIEventSystem;
    public List<Image> PlayerIconPlaceholders;

    public GameObject TrackButtonsPanel;
    public Image SelectedTrackLargeImage;
    public Text SelectedTrackName;

    private float m_RaceSetupInputDelay = 0.0f;
    public GameObject AIRacersSetting;
    private int m_AIRacers = 3;
    public GameObject DifficultySetting;
    private List<string> m_Difficulties = new List<string>() {"Easy", "Normal", "Hard" };
    private int m_SelectedDifficultyIndex = 1;
    public GameObject LapsSetting;
    private int m_LapCount = 3;

    public GameObject CharacterSelectionPrefab;

    private GameObject[] m_KartPlatforms = new GameObject[4];

    private Dictionary<string, MenuPanel> m_MenuPanels = new Dictionary<string, MenuPanel>();
    private string PrimaryInputPrefix;
    private string ActiveMenuPanel;
    private Vector2 ScreenDimensions;
    private List<GameObject> m_Tracks = new List<GameObject>();
    private List<GameObject> m_TrackButtons;
    private int TrackSelectOffset = 0;
    private GameObject m_SelectedTrackObject;

    public List<GameObject> Players = new List<GameObject>();
    List<GameObject> CharacterSelectors = new List<GameObject>();
    private GameObject m_KartCameraPrefab;

    public void Start()
    {
        m_KartPlatforms = Resources.LoadAll<GameObject>("Kart Platforms");

        m_KartCameraPrefab = Resources.Load<GameObject>("Cameras/KartCamera");

        ScreenDimensions = new Vector2(Screen.width, Screen.height);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        LoadAllTrackInfoPrefabs();

        foreach (MenuPanel l_MenuPanel in MenuPanels)
            m_MenuPanels.Add(l_MenuPanel.Name, l_MenuPanel);

        HideAllPanels();

        m_MenuPanels["TitleScreen"].UIPanel.SetActive(true);

        ActiveMenuPanel = "TitleScreen";

        Resolution[] l_Resolutions = Screen.resolutions;

        ScreenResolutionsDropdown.options = new List<UnityEngine.UI.Dropdown.OptionData>();

        for (int i = 0; i < l_Resolutions.Length; i++)
        {
            ScreenResolutionsDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData());

            ScreenResolutionsDropdown.options[i].text = l_Resolutions[i].width + " x " + l_Resolutions[i].height;

            ScreenResolutionsDropdown.value = i;
        }

        ScreenResolutionsDropdown.RefreshShownValue();
    }

    public void Update()
    {
        m_RaceSetupInputDelay -= Time.deltaTime;

        switch (ActiveMenuPanel)
        {
            case "TitleScreen":
                if (Input.GetButton("JS1Start"))
                    SetupPrimaryPlayer("JS1");
                else if (Input.GetButton("JS2Start"))
                    SetupPrimaryPlayer("JS2");
                else if (Input.GetButton("JS3Start"))
                    SetupPrimaryPlayer("JS3");
                else if (Input.GetButton("JS4Start"))
                    SetupPrimaryPlayer("JS4");
                else if (Input.GetButton("KStart"))
                    SetupPrimaryPlayer("K");
                else
                    break;
                UIEventSystem.UpdateModules();

                ChangeMenuPanel("MainMenu");
                break;

            case "CharacterSelection":
                if (Screen.width != ScreenDimensions.x || Screen.height != ScreenDimensions.y)
                    SetCharacterSelectCameras();

                ListenForPlayerDropIn();

                foreach (GameObject l_GameObject in CharacterSelectors)
                {
                    if (!l_GameObject.GetComponent<CharacterSelectionController>().PlayerReady)
                        return;
                }

                ChangeMenuPanel("TrackSelection");
                break;

            case "TrackSelection":
                if (Screen.width != ScreenDimensions.x || Screen.height != ScreenDimensions.y)
                    UpdateVisibleTrackButtons();

                TrackInfo l_TrackInfo = UIEventSystem.currentSelectedGameObject.GetComponent<TrackInfo>();

                if (l_TrackInfo != null)
                    m_SelectedTrackObject = UIEventSystem.currentSelectedGameObject;
                
                SelectedTrackLargeImage.material = m_SelectedTrackObject.GetComponent<Image>().material;
                SelectedTrackName.text = m_SelectedTrackObject.GetComponent<TrackInfo>().TrackName;
                break;

            case "RaceSetup":
                if (m_RaceSetupInputDelay <= 0)
                {
                    if (Input.GetAxis(PrimaryInputPrefix + "MenuHorizontal") > 0.8f)
                    {
                        if (UIEventSystem.currentSelectedGameObject == AIRacersSetting)
                            m_AIRacers = Mathf.Clamp(m_AIRacers + 1, 0, (12 - Players.Count));
                        else if (UIEventSystem.currentSelectedGameObject == DifficultySetting)
                            m_SelectedDifficultyIndex = Mathf.Clamp(m_SelectedDifficultyIndex + 1, 0, 2);
                        else if (UIEventSystem.currentSelectedGameObject == LapsSetting)
                            m_LapCount = Mathf.Clamp(m_LapCount + 1, 2, 7);

                        AIRacersSetting.GetComponentInChildren<Text>().text = m_AIRacers.ToString();
                        DifficultySetting.GetComponentInChildren<Text>().text = m_Difficulties[m_SelectedDifficultyIndex];
                        LapsSetting.GetComponentInChildren<Text>().text = m_LapCount.ToString();

                        m_RaceSetupInputDelay = 0.2f;
                    }
                    else if (Input.GetAxis(PrimaryInputPrefix + "MenuHorizontal") < -0.8f)
                    {
                        if (UIEventSystem.currentSelectedGameObject == AIRacersSetting)
                            m_AIRacers = Mathf.Clamp(m_AIRacers - 1, 0, (12 - Players.Count));
                        else if (UIEventSystem.currentSelectedGameObject == DifficultySetting)
                            m_SelectedDifficultyIndex = Mathf.Clamp(m_SelectedDifficultyIndex - 1, 0, 2);
                        else if (UIEventSystem.currentSelectedGameObject == LapsSetting)
                            m_LapCount = Mathf.Clamp(m_LapCount - 1, 2, 7);

                        AIRacersSetting.GetComponentInChildren<Text>().text = m_AIRacers.ToString();
                        DifficultySetting.GetComponentInChildren<Text>().text = m_Difficulties[m_SelectedDifficultyIndex];
                        LapsSetting.GetComponentInChildren<Text>().text = m_LapCount.ToString();

                        m_RaceSetupInputDelay = 0.2f;
                    }
                }
                break;
        }

        if (Screen.width != ScreenDimensions.x || Screen.height != ScreenDimensions.y)
        {
            ScreenDimensions.x = Screen.width;
            ScreenDimensions.y = Screen.height;
        }
    }
    
    private void HideAllPanels()
    {
        foreach (KeyValuePair<string, MenuPanel> l_Pair in m_MenuPanels)
        {
            l_Pair.Value.UIPanel.SetActive(false);
        }
    }

    public void ChangeMenuPanel(string p_PanelName)
    {
        HandleSpecialPanelExitConditions(ActiveMenuPanel);

        if (m_MenuPanels.ContainsKey(p_PanelName))
        {
            HideAllPanels();
            m_MenuPanels[p_PanelName].UIPanel.SetActive(true);

            ActiveMenuPanel = p_PanelName;

            UIEventSystem.SetSelectedGameObject(null);
            UIEventSystem.SetSelectedGameObject(m_MenuPanels[p_PanelName].FirstSelected);
        }

        HandleSpecialPanelEnterConditions(p_PanelName);
    }

    private void HandleSpecialPanelExitConditions(string p_PanelName)
    {
        switch (p_PanelName)
        {
            case "CharacterSelection":
                DisableCharacterSelectionControllers();

                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].GetComponent<PlayerController>().Kart = CharacterSelectors[i].GetComponent<CharacterSelectionController>().GetSelectedKart();
                    Players[i].GetComponent<PlayerController>().Character = CharacterSelectors[i].GetComponent<CharacterSelectionController>().GetSelectedCharacter();
                    Players[i].GetComponent<PlayerController>().InCarCharacter = CharacterSelectors[i].GetComponent<CharacterSelectionController>().GetSelectedInCarCharacter();
                }

                break;
        }
    }

    private void HandleSpecialPanelEnterConditions(string p_PanelName)
    {
        switch (p_PanelName)
        {
            case "CharacterSelection":
                EnableCharacterSelectionControllers();

                SetCharacterSelectCameras();
                break;
            case "TrackSelection":
                UpdateVisibleTrackButtons();
                break;
        }
    }

    public void LoadAllTrackInfoPrefabs()
    {
        GameObject[] l_Tracks = Resources.LoadAll<GameObject>("Tracks");

        foreach (GameObject l_Track in l_Tracks)
            m_Tracks.Add(l_Track);
    }

    public void MasterVolumeChanged(Slider p_VolumeSlider)
    {

    }

    public void MusicVolumeChanged(Slider p_VolumeSlider)
    {

    }

    public void SFXVolumeChanged(Slider p_VolumeSlider)
    {

    }

    public void ScreenResolutionChanged(Dropdown p_Dropdown)
    {
        Resolution l_SelectedResolution = Screen.resolutions[p_Dropdown.value];

        Screen.SetResolution(l_SelectedResolution.width, l_SelectedResolution.height, true);
    }

    private void DisableCharacterSelectionControllers()
    {
        foreach (GameObject l_GameObject in CharacterSelectors)
        {
            CharacterSelectionController l_CharacterSelectionController = l_GameObject.GetComponent<CharacterSelectionController>();

            l_CharacterSelectionController.enabled = false;
        }
    }

    private void EnableCharacterSelectionControllers()
    {
        foreach (GameObject l_GameObject in CharacterSelectors)
        {
            CharacterSelectionController l_CharacterSelectionController = l_GameObject.GetComponent<CharacterSelectionController>();

            l_CharacterSelectionController.ResetSelectedPlatformKartSkin();
            l_CharacterSelectionController.ResetSelectedPlatformRacesuitSkin();

            l_CharacterSelectionController.enabled = true;
        }
    }

    private void UpdateVisibleTrackButtons()
    {
        Debug.Log("Refreshing");

        foreach (Transform child in TrackButtonsPanel.transform)
            Destroy(child.gameObject);

        RectTransform l_TrackButtonsContainer = TrackButtonsPanel.GetComponent<RectTransform>();

        int MaximumButtons = (int)(l_TrackButtonsContainer.rect.width - 25) / 275;
        float Remainder = (l_TrackButtonsContainer.rect.width - 25) % 275;

        float ButtonSpacing = 25 + (Remainder / (MaximumButtons + 1));

        float xOffset = ButtonSpacing;

        int ButtonsOnScreen = (m_Tracks.Count - TrackSelectOffset) > MaximumButtons ? MaximumButtons : (m_Tracks.Count - TrackSelectOffset);

        for (int i = 0; i < ButtonsOnScreen; i++)
        {
            GameObject l_TrackImageObject = Instantiate(m_Tracks[i + TrackSelectOffset]);

            l_TrackImageObject.transform.SetParent(TrackButtonsPanel.transform, false);

            l_TrackImageObject.GetComponent<RectTransform>().transform.position = new Vector3(0, 0, 0);
            l_TrackImageObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, xOffset, 250);
            l_TrackImageObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 150);

            l_TrackImageObject.GetComponent<Button>().onClick.AddListener(() => TrackButtonClick());

            xOffset += (ButtonSpacing + 250);

            if (i == 0)
                UIEventSystem.SetSelectedGameObject(l_TrackImageObject);
        }
    }

    public void TrackButtonClick()
    {
        ChangeMenuPanel("RaceSetup");
    }

    public void CycleTracksLeft()
    {
        RectTransform l_TrackButtonsContainer = TrackButtonsPanel.GetComponent<RectTransform>();

        int MaximumButtonsOnScreen = (int)(l_TrackButtonsContainer.rect.width - 25) / 275;

        if (TrackSelectOffset - MaximumButtonsOnScreen >= 0)
        {
            TrackSelectOffset -= MaximumButtonsOnScreen;
        }
        UpdateVisibleTrackButtons();
    }

    public void CycleTracksRight()
    {
        RectTransform l_TrackButtonsContainer = TrackButtonsPanel.GetComponent<RectTransform>();

        int MaximumButtonsOnScreen = (int)(l_TrackButtonsContainer.rect.width - 25) / 275;
        
        if (TrackSelectOffset + MaximumButtonsOnScreen < m_Tracks.Count)
        {
            TrackSelectOffset += MaximumButtonsOnScreen;
        }
        UpdateVisibleTrackButtons();
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    void SetupPrimaryPlayer(string p_InputPrefix)
    {
        PrimaryInputPrefix = p_InputPrefix;

        AddPlayer(p_InputPrefix);

        DisableCharacterSelectionControllers();

        UIEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().horizontalAxis = p_InputPrefix + "MenuHorizontal";
        UIEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().verticalAxis = p_InputPrefix + "MenuVertical";
        UIEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().submitButton = p_InputPrefix + "Action";
        UIEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().cancelButton = p_InputPrefix + "Return";
        UIEventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().UpdateModule();
    }

    public void ListenForPlayerDropIn()
    {
        if (Input.GetButton("JS1Start"))
        {
            if (DoesControllerIDExist("JS1") == false)
                AddPlayer("JS1");
        }
        else if (Input.GetButton("JS2Start"))
        {
            if (DoesControllerIDExist("JS2") == false)
                AddPlayer("JS2");
        }
        else if (Input.GetButton("JS3Start"))
        {
            if (DoesControllerIDExist("JS3") == false)
                AddPlayer("JS3");
        }
        else if (Input.GetButton("JS4Start"))
        {
            if (DoesControllerIDExist("JS4") == false)
                AddPlayer("JS4");
        }
        else if (Input.GetButton("KStart"))
        {
            if (DoesControllerIDExist("K") == false)
                AddPlayer("K");
        }
        else
            return;

        SetCharacterSelectCameras();
    }

    private bool DoesControllerIDExist(string p_controllerID)
    {
        foreach (GameObject l_PlayerObject in Players)
        {
            PlayerController l_Player = l_PlayerObject.GetComponent<PlayerController>();

            if (l_Player.ControllerID.Trim().Equals(p_controllerID))
                return true;
        }
        return false;
    }

    public void AddPlayer(string p_ControllerID)
    {
        GameObject l_PlayerObject = new GameObject();
        l_PlayerObject.AddComponent<PlayerController>();
        l_PlayerObject.name = "Player " + Players.Count;

        PlayerController l_Player = l_PlayerObject.GetComponent<PlayerController>();
        l_Player.ControllerID = p_ControllerID;
        l_Player.Name = "Player " + Players.Count;
        
        CharacterSelectors.Add(Instantiate(CharacterSelectionPrefab));

        CharacterSelectors[CharacterSelectors.Count - 1].transform.Translate(Vector3.up * 10 * (CharacterSelectors.Count - 1));

        CharacterSelectors[CharacterSelectors.Count - 1].GetComponent<CharacterSelectionController>().PlayerInputPrefix = p_ControllerID;

        CharacterSelectors[CharacterSelectors.Count - 1].GetComponent<CharacterSelectionController>().CharacterIconImage = PlayerIconPlaceholders[CharacterSelectors.Count - 1];

        l_Player.Active = false;

        Players.Add(l_PlayerObject);

        CharacterSelectionController l_CharacterSelectionController = CharacterSelectors[Players.Count - 1].GetComponent<CharacterSelectionController>();

        l_CharacterSelectionController.ResetSelectedPlatformKartSkin();
        l_CharacterSelectionController.ResetSelectedPlatformRacesuitSkin();
        l_CharacterSelectionController.enabled = true;
    }

    private void SetCharacterSelectCameras()
    {
        float l_EdgePergentage = 100f / Screen.width;
        float l_CameraAreaWidthPercentage = 1.0f - l_EdgePergentage;
        float l_HalfCameraAreaWidthPercentage = (1.0f - l_EdgePergentage) / 2f;

        switch (Players.Count)
        {
            case 0:
                break;
            case 1:
                CharacterSelectors[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, l_CameraAreaWidthPercentage, 1f));
                break;
            case 2:
                CharacterSelectors[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, l_CameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, l_CameraAreaWidthPercentage, 0.5f));
                break;
            case 3:
                CharacterSelectors[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, l_HalfCameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(l_HalfCameraAreaWidthPercentage, 0.5f, l_HalfCameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[2].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, l_HalfCameraAreaWidthPercentage, 0.5f));
                break;
            case 4:
                CharacterSelectors[0].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0.5f, l_HalfCameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[1].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(l_HalfCameraAreaWidthPercentage, 0.5f, l_HalfCameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[2].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(0f, 0f, l_HalfCameraAreaWidthPercentage, 0.5f));
                CharacterSelectors[3].GetComponentInChildren<CameraController>().SetCameraScreenSize(new Rect(l_HalfCameraAreaWidthPercentage, 0f, l_HalfCameraAreaWidthPercentage, 0.5f));
                break;
        }
    }

    public void LoadRace()
    {
        GameObject l_TrackInfo = Instantiate(m_SelectedTrackObject);
        l_TrackInfo.name = "Track Info";
        l_TrackInfo.transform.position = new Vector3(0, 0, 0);

        GameObject l_FadeController = new GameObject();
        l_FadeController.AddComponent<ObjectFadeController>();
        l_FadeController.name = "Fade Controller";

        GameObject l_RaceManager = new GameObject();
        l_RaceManager.AddComponent<RaceManager>();
        l_RaceManager.GetComponent<RaceManager>().SetFadeController(l_FadeController.GetComponent<ObjectFadeController>());
        l_RaceManager.GetComponent<RaceManager>().SetTrackInfo(l_TrackInfo.GetComponent<TrackInfo>());
        l_RaceManager.name = "Race Manager";

        foreach (GameObject l_PlayerObject in Players)
        {
            PlayerController l_Player = l_PlayerObject.GetComponent<PlayerController>();

            l_Player.Kart.transform.SetParent(l_PlayerObject.transform);
            l_Player.Character.transform.SetParent(l_PlayerObject.transform);

            int l_KartMaterialIndex = l_Player.Kart.GetComponent<KartController>().GetMaterialSkinIndex();

            l_RaceManager.GetComponent<RaceManager>().AddPlayer(l_Player, m_KartCameraPrefab, l_KartMaterialIndex, 0);            
        }

        for (int i = 0; i < m_AIRacers; i++)
        {
            GameObject l_AIRacerObject = new GameObject();
            l_AIRacerObject.AddComponent<AIController>();

            AIController l_AIRacer = l_AIRacerObject.GetComponent<AIController>();

            int KartIndex = Random.Range(0, 4);

            Debug.Log("Kart Index: " + KartIndex);

            l_AIRacer.Kart = m_KartPlatforms[KartIndex].GetComponent<PlatformController>().Kart;
            l_AIRacer.Character = m_KartPlatforms[KartIndex].GetComponent<PlatformController>().Character;
            l_AIRacer.InCarCharacter = m_KartPlatforms[KartIndex].GetComponent<PlatformController>().InCarCharacter;

            l_RaceManager.GetComponent<RaceManager>().AddAI(l_AIRacer, Random.Range(0, 3), Random.Range(0, 3));

        }

        DontDestroyOnLoad(l_TrackInfo);
        DontDestroyOnLoad(l_FadeController);
        DontDestroyOnLoad(l_RaceManager);

        string l_SceneName = l_TrackInfo.GetComponent<TrackInfo>().TrackName;

        UnityEngine.SceneManagement.SceneManager.LoadScene(l_SceneName);        
    }
}
