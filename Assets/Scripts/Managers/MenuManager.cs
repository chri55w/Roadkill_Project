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
    public List<Image> PlayerIconGlowImages;

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

    public GameObject PlayerHUDPrefab;

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

        ScreenResolutionsDropdown.options = new List<Dropdown.OptionData>();

        for (int i = 0; i < l_Resolutions.Length; i++)
        {
            ScreenResolutionsDropdown.options.Add(new Dropdown.OptionData());

            ScreenResolutionsDropdown.options[i].text = l_Resolutions[i].width + " x " + l_Resolutions[i].height;

            ScreenResolutionsDropdown.value = i;
        }

        ScreenResolutionsDropdown.RefreshShownValue();

        for (int i = 0; i < 4; i++)
        {
            CharacterSelectors.Add(Instantiate(CharacterSelectionPrefab));

            CharacterSelectors[CharacterSelectors.Count - 1].transform.Translate(Vector3.up * 10 * (CharacterSelectors.Count - 1));

            CharacterSelectors[CharacterSelectors.Count - 1].GetComponent<CharacterSelectionController>().enabled = false;
        }

        float l_WidthEdgePergentage = 100f / Screen.width;
        float l_HeightEdgePergentage = 100f / Screen.height;
        float l_CameraAreaWidthPercentage = 1.0f - l_WidthEdgePergentage;
        float l_HalfCameraAreaWidthPercentage = (1.0f - l_WidthEdgePergentage) / 2f;
        float l_CameraAreaHeightPercentage = 1.0f - l_HeightEdgePergentage;
        float l_HalfCameraAreaHeightPercentage = (1.0f - l_HeightEdgePergentage) / 2f;

        CharacterSelectors[0].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, l_HalfCameraAreaHeightPercentage + l_HeightEdgePergentage, l_HalfCameraAreaWidthPercentage, l_HalfCameraAreaHeightPercentage));
        CharacterSelectors[1].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(l_HalfCameraAreaWidthPercentage, l_HalfCameraAreaHeightPercentage + l_HeightEdgePergentage, l_HalfCameraAreaWidthPercentage, l_HalfCameraAreaHeightPercentage));
        CharacterSelectors[2].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(0f, l_HeightEdgePergentage, l_HalfCameraAreaWidthPercentage, l_HalfCameraAreaHeightPercentage));
        CharacterSelectors[3].GetComponentInChildren<CameraTools>().SetCameraScreenSize(new Rect(l_HalfCameraAreaWidthPercentage, l_HeightEdgePergentage, l_HalfCameraAreaWidthPercentage, l_HalfCameraAreaHeightPercentage));
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
                {
                    ScreenDimensions = new Vector2(Screen.width, Screen.height);

                    SetCharacterSelectCameras();
                }

                ListenForPlayerDropIn();

                for (int i = 0; i < CharacterSelectors.Count; i++)
                {
                    if (!CharacterSelectors[i].GetComponent<CharacterSelectionController>().PlayerReady)
                        PlayerIconGlowImages[i].enabled = false;
                    else
                        PlayerIconGlowImages[i].enabled = true;
                }
                
                for (int i = 0; i < Players.Count; i++)
                {
                    if (!CharacterSelectors[i].GetComponent<CharacterSelectionController>().PlayerReady)
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
                
                SelectedTrackLargeImage.sprite = m_SelectedTrackObject.GetComponent<Image>().sprite;
                SelectedTrackName.text = m_SelectedTrackObject.GetComponent<TrackInfo>().TrackName;
                break;

            case "RaceSetup":
                if (m_RaceSetupInputDelay <= 0)
                {
                    if (Input.GetAxis(PrimaryInputPrefix + "MenuHorizontal") > 0.8f)
                    {
                        if (UIEventSystem.currentSelectedGameObject == AIRacersSetting)
                            m_AIRacers = Mathf.Clamp(m_AIRacers + 1, (4 - Players.Count), (12 - Players.Count));
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
                            m_AIRacers = Mathf.Clamp(m_AIRacers - 1, (4 - Players.Count), (12 - Players.Count));
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
        for (int i = 0; i < Players.Count; i++)
        {
            CharacterSelectionController l_CharacterSelectionController = CharacterSelectors[i].GetComponent<CharacterSelectionController>();

            l_CharacterSelectionController.ResetSelectedPlatformKartSkin();
            l_CharacterSelectionController.ResetSelectedPlatformRacesuitSkin();

            l_CharacterSelectionController.enabled = true;
        }
    }

    private void UpdateVisibleTrackButtons()
    {
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

        l_Player.Active = false;

        Players.Add(l_PlayerObject);

        CharacterSelectionController l_CharacterSelectionController = CharacterSelectors[Players.Count - 1].GetComponent<CharacterSelectionController>();

        l_CharacterSelectionController.ResetSelectedPlatformKartSkin();
        l_CharacterSelectionController.ResetSelectedPlatformRacesuitSkin();
        l_CharacterSelectionController.enabled = true;
        l_CharacterSelectionController.PlayerInputPrefix = p_ControllerID;
        l_CharacterSelectionController.CharacterIconImage = PlayerIconPlaceholders[Players.Count - 1];
    }

    private void SetCharacterSelectCameras()
    {
        float l_EdgePergentage = 100f / Screen.width;
        float l_CameraAreaWidthPercentage = 1.0f - l_EdgePergentage;
        float l_HalfCameraAreaWidthPercentage = (1.0f - l_EdgePergentage) / 2f;

        switch (Players.Count)
        {
            case 0:
                CharacterSelectors[0].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[1].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[2].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[3].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                break;
            case 1:
                CharacterSelectors[0].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[1].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[2].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[3].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                break;
            case 2:
                CharacterSelectors[0].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[1].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[2].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                CharacterSelectors[3].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                break;
            case 3:
                CharacterSelectors[0].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[1].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[2].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[3].GetComponentInChildren<CameraTools>().CameraComponent.enabled = false;
                break;
            case 4:
                CharacterSelectors[0].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[1].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[2].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
                CharacterSelectors[3].GetComponentInChildren<CameraTools>().CameraComponent.enabled = true;
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
        l_RaceManager.GetComponent<RaceManager>().SetupCanvas();
        l_RaceManager.GetComponent<RaceManager>().ConfigureMinimap();
        l_RaceManager.GetComponent<RaceManager>().RaceLaps = m_LapCount;

        for (int i = 0; i < Players.Count; i++)
        {
            PlayerController l_Player = Players[i].GetComponent<PlayerController>();

            l_Player.Kart.transform.SetParent(Players[i].transform);
            l_Player.Character.transform.SetParent(l_Player.Kart.transform);
            l_Player.Character.transform.localPosition = l_Player.Kart.transform.Find("Character Position").localPosition;
            l_Player.Character.transform.localRotation = l_Player.Kart.transform.Find("Character Position").localRotation;
            l_Player.Character.GetComponent<Character>().ChangeState(CharacterState.DRIVING);

            l_Player.CharacterIcon = CharacterSelectors[i].GetComponent<CharacterSelectionController>().GetSelectedCharacterIcon();

            int l_KartMaterialIndex = l_Player.Kart.GetComponent<KartController>().GetMaterialSkinIndex();

            l_RaceManager.GetComponent<RaceManager>().AddPlayer(Players[i], m_KartCameraPrefab, PlayerHUDPrefab);
        }

        for (int i = 0; i < m_AIRacers; i++)
        {
            GameObject l_AIRacerObject = new GameObject();
            AIController l_AIRacer = l_AIRacerObject.AddComponent<AIController>();
            l_AIRacerObject.name = "AI Driver " + i;

            int KartIndex = Random.Range(0, 4);

            l_AIRacer.Kart = Instantiate(m_KartPlatforms[KartIndex].GetComponent<PlatformController>().Kart);
            l_AIRacer.Character = Instantiate(m_KartPlatforms[KartIndex].GetComponent<PlatformController>().Character);

            l_AIRacer.Kart.GetComponent<KartController>().SetRandomKartSkin();
            l_AIRacer.Character.GetComponent<Character>().SetRandomCharacterSkin();

            l_AIRacer.Kart.transform.SetParent(l_AIRacerObject.transform);
            l_AIRacer.Character.transform.SetParent(l_AIRacer.Kart.transform);
            l_AIRacer.Character.transform.localPosition = l_AIRacer.Kart.transform.Find("Character Position").localPosition;
            l_AIRacer.Character.transform.localRotation = l_AIRacer.Kart.transform.Find("Character Position").localRotation;
            l_AIRacer.Character.GetComponent<Character>().SetupCharacter();
            l_AIRacer.Character.GetComponent<Character>().ChangeState(CharacterState.DRIVING);

            l_AIRacer.CharacterIcon = m_KartPlatforms[KartIndex].GetComponent<PlatformController>().CharacterIcon;

            l_RaceManager.GetComponent<RaceManager>().AddAI(l_AIRacerObject);
        }

        l_RaceManager.GetComponent<RaceManager>().BeginOpeningCinematic();

        DontDestroyOnLoad(l_TrackInfo);
        DontDestroyOnLoad(l_FadeController);
        DontDestroyOnLoad(l_RaceManager);

        string l_SceneName = l_TrackInfo.GetComponent<TrackInfo>().TrackName;

        UnityEngine.SceneManagement.SceneManager.LoadScene(l_SceneName);        
    }
}
