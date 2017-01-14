using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Managers;
using Controllers;

namespace Objects
{
    public abstract class Driver : MonoBehaviour
    {
        public enum e_RespawnState { ALIVE, DYING, DEAD, RESPAWNING };

        public GameObject Kart;
        public GameObject CurrentPickup;
        public string Name;

        //Respawn variables
        public float RespawnDistanceModifier = 5;
        public BezierSpline CurrentSpline;
        public int SplineDetail;
        float ClosestTimePointOnSpline = 0f;

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
        protected e_RespawnState m_RespawnState;        

        public void MakeDriver(GameObject p_Kart, Transform p_StartPosition, RaceManager p_RaceManager, BezierSpline p_StartingSpline, ObjectFadeController p_SidLordOfTheFade)
        {
            Kart = p_Kart;

            transform.position = p_StartPosition.position;
            transform.rotation = p_StartPosition.rotation;

            m_KartHealth = 3;
            
            m_RaceManager = p_RaceManager;

            m_FadeController = p_SidLordOfTheFade;

            CurrentSpline = p_StartingSpline;
            SplineDetail = CurrentSpline.MeshDetailLevel;

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
            m_RespawnState = e_RespawnState.ALIVE;
        }

        public void Start()
        {
            Name = transform.root.gameObject.name;
        }

        protected bool DeathCheck()
        {
            if (m_KartHealth <= 0)
            {
                switch(m_RespawnState)
                {
                    case e_RespawnState.DYING:
                        for (int i = 0; i < FadeIndex.Length; i++)
                            FadeIndex[i] = false;

                        Die();
                        m_RespawnState = e_RespawnState.DEAD;
                        return true;
                    case e_RespawnState.DEAD:
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
                        {
                            m_Respawning = true;
                            StartCoroutine(RespawnWithDelay(1));
                            m_RespawnState = e_RespawnState.RESPAWNING;
                        }
                        return true;
                    case e_RespawnState.RESPAWNING:
                        if (m_Respawning == false)
                        {
                            m_RespawnState = e_RespawnState.ALIVE;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                }
            }
            return false;
        }

        public void TakeDamage(int p_damage)
        {
            m_KartHealth -= p_damage;

            if (m_KartHealth <= 0)
                m_RespawnState = e_RespawnState.DYING;
        }

        protected void Die()
        {
            //Let animation ie head roll (if needed) happen
            //Fade out character (1-0)
            Faded = false;

            m_FadeController.StartMultiFade(gameObject, m_KartMaterials, m_KartColorIDs, 2.0f, 0);

            // LEWIS - Call death sounds here!

            //Turn off collision & physics ect
            //Remove current power-up
        }

        IEnumerator RespawnWithDelay(float p_TimeToWait)
        {
            yield return new WaitForSeconds(p_TimeToWait);
            StartCoroutine(Respawn());
        } 

        protected IEnumerator Respawn()
        {
            //Get respawn point
            Vector3 l_RespawnPoint = GetPointBehind(RespawnDistanceModifier);
            l_RespawnPoint.y += 1.0f;
            //Move Kart to respawn point - may need to add facing correct direction on respawn
            // Change to Lerp? - wont pass through waypoints otherwise
            Kart.transform.position = l_RespawnPoint;
            FaceForwards(l_RespawnPoint);

            //Reveal kart
            bool l_FadedBack = false;
            for (int i = 0; i < FadeIndex.Length; i++)
                FadeIndex[i] = false;
            m_FadeController.StartMultiFade(gameObject, m_KartMaterials, m_KartColorIDs, 1.0f, 1);

            int FadedCount = 0;
            for (int i = 0; i < FadeIndex.Length; i++)
            {
                if (FadeIndex[i] == true)
                    FadedCount++;
            }

            // if all fade indexes are true then object is faded
            if (FadedCount == FadeIndex.Length)
                l_FadedBack = true;


            //Restore health
            if (l_FadedBack == false)
                yield return null;

            m_KartHealth = 3;
            m_Respawning = false;
            Faded = !l_FadedBack;
            yield return null;
        }

        protected Vector3 GetPointAhead(float p_Distance)
        {
            Vector3 l_CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);

            ClosestTimePointOnSpline = CurrentSpline.GetClosestTimePointOnSpline(SplineDetail, l_CurrentPosition);

            // (+/-)(1.0f/SplineDetail) * x where x is how many steps ahead or behind you want the point to be 
            float l_TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / SplineDetail) * p_Distance)) % 1;

            Vector3 l_PointAhead = CurrentSpline.GetPoint(l_TimePointAhead);

            return l_PointAhead;
        }

        protected Vector3 GetPointBehind(float p_Distance)
        {
            Vector3 l_CurrentPosition = new Vector3(Kart.transform.position.x, Kart.transform.position.y, Kart.transform.position.z);
            
            ClosestTimePointOnSpline = CurrentSpline.GetClosestTimePointOnSpline(SplineDetail, l_CurrentPosition);
            
            // (+/-)(1.0f/SplineDetail) * x where x is how many steps ahead or behind you want the point to be 
            float l_TimePointAhead = (ClosestTimePointOnSpline - ((1.0f / SplineDetail) * p_Distance)) % 1;

            Vector3 l_PointBehind = CurrentSpline.GetPoint(l_TimePointAhead);

            return l_PointBehind;
        }

        protected void FaceForwards(Vector3 p_Postion)
        {
            ClosestTimePointOnSpline = CurrentSpline.GetClosestTimePointOnSpline(SplineDetail, p_Postion);

            // (+/-)(1.0f/SplineDetail) * x where x is how many steps ahead or behind you want the point to be 
            float l_TimePointAhead = (ClosestTimePointOnSpline + ((1.0f / SplineDetail))) % 1;

            Kart.transform.LookAt(CurrentSpline.GetPoint(l_TimePointAhead));
        }

        public void ChangeSpline(BezierSpline p_NewSpline)
        {
            CurrentSpline = p_NewSpline;
            SplineDetail = CurrentSpline.MeshDetailLevel;
        }
      }
  }