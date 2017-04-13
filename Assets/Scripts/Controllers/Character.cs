using UnityEngine;
using System.Collections.Generic;

public enum CharacterState { IDLE, DRIVING, FIRST_PLACE, SECOND_PLACE, THIRD_PLACE }

public class Character : MonoBehaviour
{
    public GameObject RacesuitObject;
    public CharacterState DefaultState = CharacterState.IDLE;
    public Animator CharacterAnimator;
    public GameObject RootObject;
    public GameObject LeftArm;
    public GameObject RightArm;
    public GameObject Head;
    public GameObject Tail;
    public string InputPrefix;
    public List<CharacterSkin> Skins;
    
    public Vector3 IdleAnimationOffset;
    public Vector3 DrivingAnimationOffset;
    public Vector3 FirstPlaceAnimationOffset;
    public Vector3 SecondPlaceAnimationOffset;
    public Vector3 ThirdPlaceAnimationOffset;

    public Vector3 m_Offset;
    private int m_SelectedSkinIndex;
    public CharacterState m_State;
    
    public void Start()
    {
        if (CharacterAnimator == null)
            SetupCharacter();
    }

    public void SetupCharacter()
    {
        CharacterAnimator = gameObject.GetComponent<Animator>();

        ChangeState(DefaultState);
    }

    void Update()
    {
        if (m_State == CharacterState.DRIVING)
        {
            CharacterAnimator.Play("Driving", 0, 0.5f);
        }
    }

    void LateUpdate()
    {
        RootObject.transform.localPosition = Vector3.zero;
    }

    public void UpdateHorizontalInput(float InputH)
    {
        CharacterAnimator.SetFloat("HorizontalInput", InputH);
    }

    public void ChangeState(CharacterState p_State)
    {
        m_State = p_State;

        if (p_State == CharacterState.IDLE || p_State == CharacterState.DRIVING)
        {
            Head.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].IdleMaterial;
            LeftArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].IdleMaterial;
            RightArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].IdleMaterial;
            Tail.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].IdleMaterial;
            RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;

            if (p_State == CharacterState.IDLE)
            {
                m_Offset = IdleAnimationOffset;
                CharacterAnimator.Play("Idle");
            }
            else if (p_State == CharacterState.DRIVING)
            {
                m_Offset = DrivingAnimationOffset;
                CharacterAnimator.Play("Driving");
            }
        }
        else if (p_State == CharacterState.FIRST_PLACE)
        {
            Head.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].FirstPlaceMaterial;
            LeftArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].FirstPlaceMaterial;
            RightArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].FirstPlaceMaterial;
            Tail.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].FirstPlaceMaterial;
            RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;

            m_Offset = FirstPlaceAnimationOffset;
            CharacterAnimator.Play("First Place");
        }
        else if (p_State == CharacterState.SECOND_PLACE)
        {
            Head.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].SecondPlaceMaterial;
            LeftArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].SecondPlaceMaterial;
            RightArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].SecondPlaceMaterial;
            Tail.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].SecondPlaceMaterial;
            RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;

            m_Offset += SecondPlaceAnimationOffset;
            CharacterAnimator.Play("Second Place");
        }
        else if (p_State == CharacterState.THIRD_PLACE)
        {
            Head.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].ThirdPlaceMaterial;
            LeftArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].ThirdPlaceMaterial;
            RightArm.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].ThirdPlaceMaterial;
            Tail.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].ThirdPlaceMaterial;
            RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;

            m_Offset += ThirdPlaceAnimationOffset;
            CharacterAnimator.Play("Third Place");
        }
    }

    public void ResetSkin()
    {
        m_SelectedSkinIndex = 0;

        Material l_CurrentMaterial = GetCurrentCharacterMaterial();

        Head.GetComponent<Renderer>().material = l_CurrentMaterial;
        LeftArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        RightArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        Tail.GetComponent<Renderer>().material = l_CurrentMaterial;

        RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;

    }

    public void ChangeSkin()
    {
        m_SelectedSkinIndex = (m_SelectedSkinIndex + 1) % Skins.Count;
        if (m_SelectedSkinIndex < 0)
            m_SelectedSkinIndex += Skins.Count;

        Material l_CurrentMaterial = GetCurrentCharacterMaterial();

        Head.GetComponent<Renderer>().material = l_CurrentMaterial;
        LeftArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        RightArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        Tail.GetComponent<Renderer>().material = l_CurrentMaterial;

        RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;
    }

    public void SetCharacterSkin(int p_SkinIndex)
    {
        if (p_SkinIndex >= 0 && Skins.Count > p_SkinIndex)
            m_SelectedSkinIndex = p_SkinIndex;

        Material l_CurrentMaterial = GetCurrentCharacterMaterial();

        Head.GetComponent<Renderer>().material = l_CurrentMaterial;
        LeftArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        RightArm.GetComponent<Renderer>().material = l_CurrentMaterial;
        Tail.GetComponent<Renderer>().material = l_CurrentMaterial;

        RacesuitObject.GetComponent<Renderer>().material = Skins[m_SelectedSkinIndex].RacesuitMaterial;
    }

    private Material GetCurrentCharacterMaterial()
    {
        switch (m_State)
        {
            case CharacterState.IDLE:
            case CharacterState.DRIVING:
                return Skins[m_SelectedSkinIndex].IdleMaterial;
            case CharacterState.FIRST_PLACE:
                return Skins[m_SelectedSkinIndex].FirstPlaceMaterial;
            case CharacterState.SECOND_PLACE:
                return Skins[m_SelectedSkinIndex].SecondPlaceMaterial;
            case CharacterState.THIRD_PLACE:
                return Skins[m_SelectedSkinIndex].ThirdPlaceMaterial;
            default:
                return Skins[m_SelectedSkinIndex].IdleMaterial;
        }
    }

    public void SetRandomCharacterSkin()
    {
        int l_SkinCount = Skins.Count;

        int l_Random = Random.Range(0, l_SkinCount);
        
        SetCharacterSkin(l_Random);
    }

    public void SetCharacterPodiumAnimation(int p_PositionIndex)
    {
        switch(p_PositionIndex)
        {
            case 1:
                ChangeState(CharacterState.FIRST_PLACE);
                break;
            case 2:
                ChangeState(CharacterState.SECOND_PLACE);
                break;
            case 3:
                ChangeState(CharacterState.THIRD_PLACE);
                break;
            default:
                ChangeState(CharacterState.IDLE);
                break;
        }
    }
}