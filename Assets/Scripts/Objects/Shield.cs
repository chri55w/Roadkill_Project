using UnityEngine;
using System.Collections;

namespace Objects
{
    public class Shield : MonoBehaviour
    {
        public GameObject ShieldCollider;
        public ParticleSystem ShieldEffect;
        public GameObject Driver;
        public float lifetime;
 
        void OnEnable()
        {
            Transform Shield = transform.GetComponentInParent<Driver>().Kart.transform.Find("VFX/Shield");
            ShieldEffect = Shield.gameObject.GetComponent<ParticleSystem>();
            transform.GetComponentInParent<Driver>().IsShielded = true;
            ShieldCollider.SetActive(true);
            ShieldEffect.Play(true);
            gameObject.SetActive(true);
        }
        
        void Update()
        {
            if (lifetime <= 0)
            {
                ShieldEffect.Stop();
                ShieldCollider.SetActive(false);
                Driver.GetComponent<Driver>().IsShielded = false;
                enabled = false;
            }                      
        }
        
        void FixedUpdate()
        {
            if(lifetime > 0)
                lifetime -= Time.fixedDeltaTime;
        } 

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            Transform l_Root = p_OtherCollider.transform.root;
            if (l_Root != null)
            {
                if (l_Root.name.Contains("Driver"))
                {
                    p_OtherCollider.GetComponentInParent<Rigidbody>().velocity = Vector3.zero;
                    p_OtherCollider.GetComponentInParent<Rigidbody>().AddForce(-p_OtherCollider.transform.forward * 500);
                }
                else if(l_Root.name.Contains("Axe"))
                {
                    Destroy(p_OtherCollider.gameObject);
                }
            }
            else
            {
                Destroy(p_OtherCollider.gameObject);
            }
        }
    }

}