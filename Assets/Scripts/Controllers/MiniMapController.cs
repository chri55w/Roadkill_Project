using UnityEngine;
using System.Collections.Generic;

public class MiniMapController : MonoBehaviour
{
    private Vector2 m_CharacterOffset = new Vector2(0, 0);
    private Vector2 m_MapSize = new Vector2(1,1);
    private Vector2 m_PositionScalingFactor = new Vector2(1,1);
    private Vector2 m_CharacterIconSize = new Vector2(24, 24);
    private List<GameObject> m_CharacterIcons = new List<GameObject>();
    private List<Objects.Driver> m_Drivers = new List<Objects.Driver>();

    void Start()
    {
        CalculateScaling();

        LoadCharacterIcons();
    }

    public void AddComponents()
    {
        gameObject.AddComponent<RectTransform>();

        gameObject.AddComponent<UnityEngine.UI.Image>();
    }
    
	void Update ()
    {
        for (int i = 0; i < m_Drivers.Count; i++)
        {
            float l_ScaledPositionX = (m_Drivers[i].Kart.transform.position.x / m_PositionScalingFactor.x) + m_CharacterOffset.x;
            float l_ScaledPositionY = (m_Drivers[i].Kart.transform.position.z / m_PositionScalingFactor.y) + m_CharacterOffset.y;

            Debug.Log(l_ScaledPositionX + " | " + l_ScaledPositionY);

            m_CharacterIcons[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, l_ScaledPositionY, m_CharacterIconSize.y);
            m_CharacterIcons[i].GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, l_ScaledPositionX, m_CharacterIconSize.x);
        }
	}

    public void SetCharacterOffset(Vector2 p_Offset)
    {
        m_CharacterOffset = p_Offset;
    }

    public void SetMapSize(Vector2 p_MapSize)
    {
        m_MapSize = p_MapSize;

        CalculateScaling();
    }

    public void SetTrackImage(Sprite p_TrackMapSprite)
    {
        gameObject.GetComponent<UnityEngine.UI.Image>().sprite = p_TrackMapSprite;

        CalculateScaling();
    }

    public void SetTrackImageSize(Vector2 p_TrackImageSize)
    {
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, p_TrackImageSize.x);
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, p_TrackImageSize.y);

        CalculateScaling();
    }
    
    public void SetScreenPosition(RectTransform.Edge p_VerticalEdge, RectTransform.Edge p_HorizontalEdge, Vector2 p_ScreenPosition, Vector2 p_TrackImageSize)
    {
        gameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(p_VerticalEdge, p_ScreenPosition.y, p_TrackImageSize.y);
        gameObject.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(p_HorizontalEdge, p_ScreenPosition.x, p_TrackImageSize.x);
        
        CalculateScaling();
    }

    public void AddDriver(Objects.Driver p_Driver)
    {
        m_Drivers.Add(p_Driver);

        LoadCharacterIcons();
    }

    public void SetCharacterIconSize(Vector2 p_CharacterIconSize)
    {
        m_CharacterIconSize = p_CharacterIconSize;

        LoadCharacterIcons();
    }

    void LoadCharacterIcons()
    {
        foreach (GameObject l_GameObject in m_CharacterIcons)
        {
            DestroyImmediate(l_GameObject);
        }

        m_CharacterIcons.Clear();

        RectTransform l_RectTransform = gameObject.GetComponent<RectTransform>();

        for (int i = 0; i < m_Drivers.Count; i++)
        {
            GameObject l_CharacterIcon = new GameObject();

            l_CharacterIcon.transform.SetParent(l_RectTransform.transform);

            l_CharacterIcon.name = "Driver Icon";

            l_CharacterIcon.AddComponent<UnityEngine.UI.Image>();

            l_CharacterIcon.GetComponent<UnityEngine.UI.Image>().sprite = m_Drivers[i].CharacterIcon;

            l_CharacterIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_CharacterIconSize.x);
            l_CharacterIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_CharacterIconSize.y);

            m_CharacterIcons.Add(l_CharacterIcon);
        }
    }

    void CalculateScaling()
    {
        RectTransform l_RectTransform = gameObject.GetComponent<RectTransform>();

        float l_ScalingFactorX = m_MapSize.x / l_RectTransform.rect.width;
        float l_ScalingFactorY = m_MapSize.y / l_RectTransform.rect.height;

        m_PositionScalingFactor = new Vector2(l_ScalingFactorX, l_ScalingFactorY);

    }
}
