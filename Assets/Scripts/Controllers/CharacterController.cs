using UnityEngine;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {

    public List<SkinnableObject> RaceSuitPeices;
    public int RaceSuitSkinMaterialID = 0;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public int GetMaterialSkinIndex()
    {
        return RaceSuitPeices[0].MaterialIndex;
    }

    public void SetRacesuitSkin(int p_MaterialIndex)
    {
        foreach(SkinnableObject l_Object in RaceSuitPeices)
        {
            l_Object.SetSkin(p_MaterialIndex);
        }
    }

    public void ChangeRacesuitSkin()
    {
        foreach (SkinnableObject l_Object in RaceSuitPeices)
        {
            l_Object.NextSkin();
        }
    }
    
    public void ResetRacesuitSkin()
    {
        foreach (SkinnableObject l_Object in RaceSuitPeices)
        {
            l_Object.ResetSkin();
        }
    }
}
