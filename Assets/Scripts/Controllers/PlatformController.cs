using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour
{
    public Sprite CharacterIcon;
    
    public GameObject Kart;
    public GameObject Character;

    void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    public void ChangeKartSkin()
    {
        Kart.GetComponent<Controllers.KartController>().ChangeKartSkin();
    }

    public void ChangeCharacterSkin()
    {
        Character.GetComponent<Character>().ChangeSkin();
    }

    public void ResetKartSkin()
    {
        Kart.GetComponent<Controllers.KartController>().ResetKartSkin();
    }

    public void ResetCharacterSkin()
    {
        Character.GetComponent<Character>().ResetSkin();
    }
}
