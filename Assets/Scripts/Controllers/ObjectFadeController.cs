using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Events;
using Objects;

//public delegate void ObjectFadeComplete(FadeObjectEventArgs e_EventArgs);

public class ObjectFadeController : MonoBehaviour
{
    //public event ObjectFadeComplete OnFadeIn;
    //public event ObjectFadeComplete OnFadeOut;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_ObjectToFade"></param>
    /// <param name="p_ColourShaderID"></param>
    /// <param name="p_FadeTime"></param>
    /// <param name="p_FadeType"> 0 == Out 1 == in </param>
    public void StartSingleFade(GameObject p_ObjectToFade, int p_ColourShaderID, float p_FadeTime, int p_FadeType)
    {
        Material l_ObjectMaterial = p_ObjectToFade.GetComponent<MeshRenderer>().material;

        if (p_FadeType == 0)
            StartCoroutine(FadeOut(p_ObjectToFade, l_ObjectMaterial, p_ColourShaderID, p_FadeTime));
        else if (p_FadeType == 1)
            StartCoroutine(FadeIn(p_ObjectToFade, l_ObjectMaterial, p_ColourShaderID, p_FadeTime));

    }

    public void StartMultiFade(GameObject p_ObjectToFade, Dictionary<int, Material> p_MaterialDictionary, int p_ColourShaderID, float p_FadeTime, int p_FadeType)
    {
        StartCoroutine(FadeGroup(p_ObjectToFade, p_MaterialDictionary, p_ColourShaderID, p_FadeTime, p_FadeType));
    }

    private IEnumerator FadeIn(GameObject p_ObjectToFade, Material p_Material, int p_ColourShaderID, float p_FadeTime)
    {
        Color l_AkuAku = p_Material.GetColor(p_ColourShaderID);
        //Debug.Log(string.Format("START Fading to {0} from {1}", p_FadeTo, l_AkuAku.a));
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / p_FadeTime)
        {
            float l_NewAlpha = Mathf.Lerp(l_AkuAku.a, 1.0f, t);
            //Debug.Log(l_NewAlpha + " at time " + t);
            p_Material.SetColor(p_ColourShaderID, new Color(l_AkuAku.r, l_AkuAku.g, l_AkuAku.b, l_NewAlpha));

            yield return null;
        }
        //Debug.Log("END FADING");
        // Bools are structs and refs cannot be passed into IEnumerators
        // Therefor must find the bool (if required) from the object directly

        //Wont work for group
        //if (OnFadeIn != null)
        //    OnFadeIn(new FadeObjectEventArgs(p_Material));

        yield return null;
    }

    /// <summary>
    /// Overload of standard FadeIn, this has a material index to work with fading a group 
    /// </summary>
    /// <param name="p_ObjectToFade"></param>
    /// <param name="p_Material"></param>
    /// <param name="p_MaterialID">Id of material if fading a group</param>
    /// <param name="p_ColourShaderID"></param>
    /// <param name="p_FadeTime"></param>
    /// <returns></returns>
    private IEnumerator FadeIn(GameObject p_ObjectToFade, Material p_Material, int p_MaterialID, int p_ColourShaderID, float p_FadeTime)
    {
        Color l_AkuAku = p_Material.GetColor(p_ColourShaderID);
        //Debug.Log(string.Format("START Fading in from {0}", l_AkuAku.a));
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / p_FadeTime)
        {
            float l_NewAlpha = Mathf.Lerp(l_AkuAku.a, 1.0f, t);
            //Debug.Log(l_NewAlpha + " at time " + t);
            p_Material.SetColor(p_ColourShaderID, new Color(l_AkuAku.r, l_AkuAku.g, l_AkuAku.b, l_NewAlpha));

            yield return null;
        }
        //Debug.Log("END FADING");
        // Bools are structs and refs cannot be passed into IEnumerators
        // Therefor must find the bool (if required) from the object directly
        if (p_ObjectToFade.name.Contains("Driver"))
        {
            p_ObjectToFade.GetComponent<Driver>().FadeIndex[p_MaterialID] = true;
        }

        //Wont work for group
        //if (OnFadeIn != null)
        //    OnFadeIn(new FadeObjectEventArgs(p_Material));

        yield return null;
    }

    private IEnumerator FadeOut(GameObject p_ObjectToFade, Material p_Material, int p_ColourShaderID, float p_FadeTime)
    {

        Color l_AkuAku = p_Material.GetColor(p_ColourShaderID);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / p_FadeTime)
        {
            float l_NewAlpha = Mathf.Lerp(l_AkuAku.a, 0.0f, t);
            //Debug.Log(l_NewAlpha + " at time " + t);
            p_Material.SetColor(p_ColourShaderID, new Color(l_AkuAku.r, l_AkuAku.g, l_AkuAku.b, l_NewAlpha));

            yield return null;
        }

        //p_FadeComplete = true;
        yield return null;
    }

    /// <summary>
    /// Overload of standard fade out to be used with a group of materials
    /// </summary>
    /// <param name="p_ObjectToFade"></param>
    /// <param name="p_Material"></param>
    /// <param name="p_MaterialID"></param>
    /// <param name="p_ColourShaderID"></param>
    /// <param name="p_FadeTime"></param>
    /// <returns></returns>
    private IEnumerator FadeOut(GameObject p_ObjectToFade, Material p_Material, int p_MaterialID, int p_ColourShaderID, float p_FadeTime)
    {
        Color l_AkuAku = p_Material.GetColor(p_ColourShaderID);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / p_FadeTime)
        {
            float l_NewAlpha = Mathf.Lerp(l_AkuAku.a, 0.0f, t);
            p_Material.SetColor(p_ColourShaderID, new Color(l_AkuAku.r, l_AkuAku.g, l_AkuAku.b, l_NewAlpha));

            yield return null;

        }
        // Bools are structs and refs cannot be passed into IEnumerators
        // Therefor must find the bool (if required) from the object directly
        if (p_ObjectToFade.name.Contains("Driver"))
        {
            p_ObjectToFade.GetComponent<Driver>().FadeIndex[p_MaterialID] = true;
        }
        //p_FadeComplete = true;
        yield return null;
    }

    // FadeMode 0 == FadeOut, 1 == FadeIn
    // Overload with no bool?
    private IEnumerator FadeGroup(GameObject p_ObjectToFade ,Dictionary<int, Material> p_MaterialDictionary, int p_ShaderID, float p_FadeTime, int p_FadeMode)
    {               
        // Start fading the materials simultaneously, based on FadeMode
        if (p_FadeMode == 0)
            for (int i = 0; i < p_MaterialDictionary.Count; i++)
                StartCoroutine(FadeOut(p_ObjectToFade, p_MaterialDictionary[i], i, p_ShaderID, p_FadeTime));
        else if (p_FadeMode == 1)
            for (int i = 0; i < p_MaterialDictionary.Count; i++)
                StartCoroutine(FadeIn(p_ObjectToFade, p_MaterialDictionary[i], i, p_ShaderID, p_FadeTime));

        // Wait for a  time just after fade time to allow objects to fade
        yield return new WaitForSeconds(p_FadeTime + 2);
    }
}
