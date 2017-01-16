using UnityEngine;
using System.Collections.Generic;

public class CharacterSelectionController : MonoBehaviour {

    public bool PlayerReady = false;

    public string PlayerInputPrefix = "";
    public GameObject SelectionPlatform;
    public List<GameObject> CharacterPlatforms;
    public UnityEngine.UI.Image CharacterIconImage;

    private float m_SwitchRotation;
    private int m_SelectedPlatformIndex;

    private Vector3 StartingRotation;
    private Vector3 TargetRotation;
    private Vector3 ActualRotation;
    private float TimeValue;

    private float IdleRotationSpeed = 0.5f;
    private float PlayerRotationSpeed = 2.0f;
        
    void Start()
    {
        m_SwitchRotation = 360 / CharacterPlatforms.Count;
    }

    public GameObject GetSelectedKart()
    {
        return CharacterPlatforms[m_SelectedPlatformIndex].GetComponent<PlatformController>().Kart;
    }

    public GameObject GetSelectedCharacter()
    {
        return CharacterPlatforms[m_SelectedPlatformIndex].GetComponent<PlatformController>().Character;
    }
    public GameObject GetSelectedInCarCharacter()
    {
        return CharacterPlatforms[m_SelectedPlatformIndex].GetComponent<PlatformController>().InCarCharacter;
    }

    void Update()
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        CharacterIconImage.material = l_SelectedCharacterPlatform.GetComponent<PlatformController>().CharacterIcon;

        if ((int)SelectionPlatform.transform.eulerAngles.y != (int)TargetRotation.y)
        {
            TimeValue = Mathf.Clamp01(TimeValue += Time.deltaTime);

            float angle = Mathf.Lerp(0, ActualRotation.y, TimeValue);

            SelectionPlatform.transform.eulerAngles = StartingRotation + (Vector3.up * angle);

            return;
        }

        if (PlayerReady)
        {
            RotateSelectedPlatform(IdleRotationSpeed);

            if (Input.GetButtonDown(PlayerInputPrefix + "Return"))
                PlayerReady = false;

            return;
        }

        if (Input.GetButtonDown(PlayerInputPrefix + "Action"))
            PlayerReady = true;

        if (Input.GetButtonDown(PlayerInputPrefix + "MenuCycleButtonOne"))
            ChangeSelectedPlatformRacesuitSkin();

        if (Input.GetButtonDown(PlayerInputPrefix + "MenuCycleButtonTwo"))
            ChangeSelectedPlatformKartSkin();

        for (int i = 0; i < CharacterPlatforms.Count; i++)
        {
            if (i != m_SelectedPlatformIndex)
            {
                GameObject l_CharacterPlatform = CharacterPlatforms[i];

                l_CharacterPlatform.transform.localRotation = Quaternion.Euler(0, 0, 0);

                l_CharacterPlatform.GetComponent<PlatformController>().ResetKartSkin();
                l_CharacterPlatform.GetComponent<PlatformController>().ResetRacesuitSkin();
            }
        }

        float l_MenuHorizontalInput = Input.GetAxis(PlayerInputPrefix + "MenuHorizontal");

        if (l_MenuHorizontalInput >= 0.8f)
            RotateSelection(1);
        else if (l_MenuHorizontalInput <= -0.8f)
            RotateSelection(-1);

        float l_CameraHorizontalInput = Input.GetAxis(PlayerInputPrefix + "CameraHorizontal");

        if (l_CameraHorizontalInput > 0.1f || l_CameraHorizontalInput < -0.1f)
            RotateSelectedPlatform(-1 * l_CameraHorizontalInput * PlayerRotationSpeed);
        else
            RotateSelectedPlatform(IdleRotationSpeed);
    }

    void RotateSelection(int RotationDirection)
    {
        m_SelectedPlatformIndex = (m_SelectedPlatformIndex + RotationDirection) % CharacterPlatforms.Count;
        if (m_SelectedPlatformIndex < 0)
            m_SelectedPlatformIndex += CharacterPlatforms.Count;
        
        StartingRotation = SelectionPlatform.transform.eulerAngles;
        float RotationAngle = RotationDirection * m_SwitchRotation;
        ActualRotation = Vector3.up * RotationAngle;

        float TargetAngle = SelectionPlatform.transform.eulerAngles.y + RotationAngle;

        if (TargetAngle < 0)
            TargetAngle += 360;
        else if (TargetAngle >= 360)
            TargetAngle -= 360;

        TargetRotation = Vector3.up * TargetAngle;

        TimeValue = 0;
    }

    void RotateSelectedPlatform(float p_RotationValue)
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        l_SelectedCharacterPlatform.transform.Rotate(Vector3.up, p_RotationValue);
    }

    public void ChangeSelectedPlatformKartSkin()
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        l_SelectedCharacterPlatform.GetComponent<PlatformController>().ChangeKartSkin();

    }

    public void ChangeSelectedPlatformRacesuitSkin()
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        l_SelectedCharacterPlatform.GetComponent<PlatformController>().ChangeRacesuitSkin();
    }

    public void ResetSelectedPlatformKartSkin()
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        l_SelectedCharacterPlatform.GetComponent<PlatformController>().ResetKartSkin();
    }

    public void ResetSelectedPlatformRacesuitSkin()
    {
        GameObject l_SelectedCharacterPlatform = CharacterPlatforms[m_SelectedPlatformIndex];

        l_SelectedCharacterPlatform.GetComponent<PlatformController>().ResetRacesuitSkin();

    }
}
