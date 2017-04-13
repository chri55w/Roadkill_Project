using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

    public Image PlayerPosition;
    public Image LapNumber;
    public Image CharacterIcon;
    public Image Speedometer;
    public Image PickupIcon;
    
    public void UpdatePlayerPosition(Sprite p_Sprite)
    {
        PlayerPosition.sprite = p_Sprite;
    }

    public void UpdateLapNumber(Sprite p_Sprite)
    {
        LapNumber.sprite = p_Sprite;
    }

    public void UpdateCharacterIcon(Sprite p_Sprite)
    {
        CharacterIcon.sprite = p_Sprite;
    }

    public void UpdateSpeedometer(float SpeedPercentage)
    {
        float l_Rotation = (Mathf.Clamp01(SpeedPercentage) * 180.0f) - 180.0f;

        Speedometer.transform.localRotation = Quaternion.AngleAxis(l_Rotation, Vector3.forward);

        //Rotate SpeedometerFill to Represent Speed
    }

    public void UpdatePickupIcon(Sprite p_Sprite)
    {
        PickupIcon.sprite = p_Sprite;
    }
}
