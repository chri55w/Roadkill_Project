using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    public class Mine : MonoBehaviour
    {
        public float radius = 5.0f;
        public float power = 10.0f;
        public float upwardModifer = 3.0f;
        public List<MeshRenderer> m_Meshes = new List<MeshRenderer>();

        private Vector3 m_ExplosionPosition;
        private SphereCollider m_SphereCollider;        
        private float m_ActivationTimer = 2.0f;
        private bool m_Triggered = false;

        void OnEnable()
        {
            m_ExplosionPosition = transform.position;
            m_SphereCollider = GetComponent<SphereCollider>();
            foreach(MeshRenderer l_mr in m_Meshes)
                l_mr.enabled = true;
        }

        void FixedUpdate()
        {
            if (m_ActivationTimer > 0)
                m_ActivationTimer -= Time.deltaTime;

            m_SphereCollider.enabled = m_ActivationTimer > 0 ? false : true;

        }

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            Collider[] l_Colliders = Physics.OverlapSphere(m_ExplosionPosition, radius);

            foreach (Collider l_hit in l_Colliders)
            {
                if (l_hit.name.Contains("Driver"))
                {
                    l_hit.transform.root.gameObject.GetComponentInChildren<Rigidbody>().AddExplosionForce(power, m_ExplosionPosition, radius, upwardModifer);
                    l_hit.transform.root.gameObject.GetComponentInChildren<Rigidbody>().AddForce((power/5) * -l_hit.transform.root.gameObject.transform.GetChild(0).transform.forward, ForceMode.Impulse);
                    m_Triggered = true;
                }
            }

            // Add explosion effect
            if (m_Triggered)
                Destroy(gameObject);
        }
    }
}