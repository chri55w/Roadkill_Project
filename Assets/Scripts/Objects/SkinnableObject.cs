using UnityEngine;
using System.Collections.Generic;

public class SkinnableObject : MonoBehaviour
{
    public List<Material> Materials;

    public int MaterialIndex = 0;
    
    public void ResetSkin()
    {
        MaterialIndex = 0;

        this.GetComponent<Renderer>().sharedMaterial = Materials[MaterialIndex];
    }

    public void NextSkin()
    {
        MaterialIndex = (MaterialIndex + 1) % Materials.Count;
        if (MaterialIndex < 0)
            MaterialIndex += Materials.Count;

        this.GetComponent<Renderer>().sharedMaterial = Materials[MaterialIndex];
    }

    public void SetSkin(int p_MaterialIndex)
    {
        p_MaterialIndex = p_MaterialIndex % Materials.Count;
        if (p_MaterialIndex < 0)
            p_MaterialIndex += Materials.Count;

        MaterialIndex = p_MaterialIndex;

        this.GetComponent<Renderer>().sharedMaterial = Materials[MaterialIndex];
    }

}
