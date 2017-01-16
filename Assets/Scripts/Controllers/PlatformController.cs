using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour
{
    public Material CharacterIcon;
    
    public GameObject Kart;
    public GameObject Character;
    public GameObject InCarCharacter;

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

    public void ChangeRacesuitSkin()
    {
        Character.GetComponent<CharacterController>().ChangeRacesuitSkin();
        InCarCharacter.GetComponent<CharacterController>().ChangeRacesuitSkin();
    }

    public void ResetKartSkin()
    {
        Kart.GetComponent<Controllers.KartController>().ResetKartSkin();
    }

    public void ResetRacesuitSkin()
    {
        Character.GetComponent<CharacterController>().ResetRacesuitSkin();
        InCarCharacter.GetComponent<CharacterController>().ResetRacesuitSkin();
    }
}
