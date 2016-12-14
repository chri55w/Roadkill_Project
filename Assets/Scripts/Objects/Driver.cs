using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Managers;
using Controllers;

namespace Objects
{
    public abstract class Driver : MonoBehaviour
    {
        public GameObject Kart;
        public GameObject CurrentPickup;
        public string Name;

        //Respawn variables
        public float RespawnDistanceModifier = 1.5f;
        public BezierSpline RespawnCenterPath;
        public int RespawnSplineDetail = 200;
        float ClosestTimeRespawnPointOnSpline = 0f;

        public ObjectFadeController m_FadeController;
        public bool Faded = false;
        public bool[] FadeIndex;
        protected RaceManager m_RaceManager;
        [SerializeField]
        protected int m_KartHealth;
        protected bool m_Respawning = false;
        // Dictionaries could be better????
        protected Dictionary<int, Material> m_KartMaterials = new Dictionary<int, Material>();
        protected int m_KartColorIDs = -1;
        protected List<GameObject> m_KartParts = new List<GameObject>();
 

        public void MakeDriver(GameObject p_Kart, Transform p_StartPosition, RaceManager p_RaceManager, BezierSpline p_StartingSpline, ObjectFadeController p_SidLordOfTheFade)
        {
            Kart = p_Kart;

            transform.position = p_StartPosition.position;
            transform.rotation = p_StartPosition.rotation;

            m_KartHealth = 3;
            
            m_RaceManager = p_RaceManager;

            m_FadeController = p_SidLordOfTheFade;

            RespawnCenterPath = p_StartingSpline;

            m_KartColorIDs = Shader.PropertyToID("_Color");
            Debug.Log("Shader ID for color: " + m_KartColorIDs);

            // Get each child object of the mesh gameObject
            for (int i = 0; i < Kart.transform.GetChild(0).childCount; i++)
            {
                m_KartParts.Add(Kart.transform.GetChild(0).GetChild(i).gameObject);
                Material m_KartPartMaterial = m_KartParts[i].GetComponent<MeshRenderer>().material;
                // 0f - opacity, 1f - cutout, 2f - fade, 3f - transparent
                m_KartPartMaterial.SetFloat("_Mode", 3f);
                //m_KartMaterialColours.Add(m_KartMaterials[i].GetColor("_Color"));

                //May need to do elsewhere
                m_KartPartMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m_KartPartMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m_KartPartMaterial.SetInt("_ZWrite", 1);
                m_KartPartMaterial.DisableKeyword("_ALPHATEST_ON");
                m_KartPartMaterial.EnableKeyword("_ALPHABLEND_ON");
                m_KartPartMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m_KartPartMaterial.renderQueue = 3000;

                m_KartMaterials.Add(i, m_KartPartMaterial);
            }

            FadeIndex = new bool[m_KartMaterials.Count];
        }


        public void Start()
        {
            Name = transform.root.gameObject.name;
        }

        protected bool DeathCheck()
        {
            if (m_KartHealth <= 0)
            {
                //Debug.Log("Checking Respawn");
                if (m_Respawning == false)
                {
                    // Die - Animation? Depends on what killed the player
                    for (int i = 0; i < FadeIndex.Length; i++)
                        FadeIndex[i] = false;

                    Die();
                    Debug.Log("I died RIP");
                    // Start respawning if not still fading
                    m_Respawning = true;
                    return true;
                }
                else
                {
                    //Raise tombstone?
                    // Check to see a material is faded
                    // if faded increase fade count
                    int FadedCount = 0;
                    for (int i = 0; i < FadeIndex.Length; i++)
                    {
                        if (FadeIndex[i] == true)
                            FadedCount++;
                    }

                    // if all fade indexes are true then object is faded
                    if (FadedCount == FadeIndex.Length)
                        Faded = true;

                    if (Faded)
                        StartCoroutine(RespawnWithDelay(1));
                    return true;
                }
            }
            else
            {
                return false; // Congratulations your alive... for now...
            }
        }

        public void TakeDamage(int p_damage)
        {
            m_KartHealth -= p_damage;

            Debug.Log("They got me");
        }

        protected void Die()
        {
            //Let animation ie head roll (if needed) happen
            //Fade out character (1-0)
            Faded = false;

            m_FadeController.StartMultiFade(gameObject, m_KartMaterials, m_KartColorIDs, 2.0f, 0);

            //Turn off collision & physics ect
            //Remove current power-up
        }

        //IEnumerator Fade(int p_MaterialIndex, float p_FadeTo, float p_FadeTime)
        //{
        //    p_FadeTo = Mathf.Clamp01(p_FadeTo);
           
        //    Color l_AkuAku = m_KartMaterials[p_MaterialIndex].GetColor(m_KartColorIDs);
        //    //Debug.Log(string.Format("START Fading to {0} from {1}", p_FadeTo, l_AkuAku.a));
        //    for (float t = 0.0f; t<1.0f; t += Time.deltaTime/p_FadeTime)
        //    {
        //        float l_NewAlpha = Mathf.Lerp(l_AkuAku.a, p_FadeTo, t);
        //        //Debug.Log(l_NewAlpha + " at time " + t);
        //        m_KartMaterials[p_MaterialIndex].SetColor(m_KartColorIDs, new Color(l_AkuAku.r, l_AkuAku.g, l_AkuAku.b, l_NewAlpha));
        //        //m_KartMaterialColours[p_MaterialIndex] = new Color(m_KartMaterialColours[p_MaterialIndex].r, m_KartMaterialColours[p_MaterialIndex].g, m_KartMaterialColours[p_MaterialIndex].b, l_NewAlpha);
        //        //m_KartMaterials[p_MaterialIndex].SetColor(Shader))
        //    }
        //    //Debug.Log("END FADING");
        //    yield return null;
        //}

        IEnumerator RespawnWithDelay(float p_TimeToWait)
        {
            yield return new WaitForSeconds(p_TimeToWait);
            StartCoroutine(Respawn());
        } 

        protected IEnumerator Respawn()
        {
            //Get respawn point
            Vector3 l_RespawnPoint = GetRespawnPoint();
            //Move Kart to respawn point - may need to add facing correct direction on respawn
            Kart.transform.position = l_RespawnPoint;

            //Reveale kart
            bool l_FadedBack = false;
            for (int i = 0; i < FadeIndex.Length; i++)
                FadeIndex[i] = false;
            m_FadeController.StartMultiFade(gameObject, m_KartMaterials, m_KartColorIDs, 3.0f, 1);

            while (l_FadedBack == false)
            {
                int FadedCount = 0;
                for (int i = 0; i < FadeIndex.Length; i++)
                {
                    if (FadeIndex[i] == true)
                        FadedCount++;
                }

                // if all fade indexes are true then object is faded
                if (FadedCount == FadeIndex.Length)
                    l_FadedBack = true;
            }

            //Restore health
            m_KartHealth = 3;
            m_Respawning = false;
            Faded = !l_FadedBack;

            yield return null;
        }

        protected Vector3 GetRespawnPoint()
        {
            Vector3 l_CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            ClosestTimeRespawnPointOnSpline = RespawnCenterPath.GetClosestTimePointOnSpline(RespawnSplineDetail, l_CurrentPosition);
            
            // (+/-)(1.0f/SplineDetail) * x where x is how many steps ahead or behind you want the point to be 
            float l_TimePointAhead = (ClosestTimeRespawnPointOnSpline - ((1.0f / RespawnSplineDetail) * RespawnDistanceModifier)) % 1;

            Vector3 l_RespawnPoint = RespawnCenterPath.GetPoint(l_TimePointAhead);

            l_RespawnPoint.y += 3.0f;

            return l_RespawnPoint;
        }
      }
  }