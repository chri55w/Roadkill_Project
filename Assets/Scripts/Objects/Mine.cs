using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    public class Mine : MonoBehaviour
    {
        public int damage = 1;
        public float radius = 5.0f;
        public float power = 10.0f;
        public float upwardModifer = 3.0f;
        public GameObject MeshObject;
        public ParticleSystem ExplosionEffect;

        private Vector3 m_ExplosionPosition;
        private SphereCollider m_SphereCollider;        
        private float m_ActivationTimer = 2.0f;
        private bool m_Triggered = false;

        void OnEnable()
        {
            m_ExplosionPosition = transform.position;
            m_SphereCollider = GetComponent<SphereCollider>();
            MeshObject.SetActive(true);
        }

        void Update()
        {
            if(m_Triggered)
            {
                m_SphereCollider.enabled = false;

                if(!ExplosionEffect.IsAlive(true))
                    Destroy(gameObject);
            }

        }

        void FixedUpdate()
        {
            if (m_ActivationTimer > 0)
                m_ActivationTimer -= Time.deltaTime;

            m_SphereCollider.enabled = m_ActivationTimer > 0 ? false : true;
        }

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            if (!m_Triggered)
            {
                Collider[] l_Colliders = Physics.OverlapSphere(m_ExplosionPosition, radius);

                GameObject l_HitObject = new GameObject();

                foreach (Collider l_hit in l_Colliders)
                {
                    if (l_hit.name.Contains("Driver"))
                    {
                        l_HitObject = l_hit.transform.root.gameObject;
                        l_HitObject.GetComponent<Driver>().TakeDamage(damage);
                        l_HitObject.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                        l_HitObject.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * power, ForceMode.VelocityChange);
                        m_Triggered = true;
                    }
                }

                if (m_Triggered)
                {
                    ExplosionEffect.Play(true);
                    MeshObject.SetActive(false);
                }
            }
        }
    }
}