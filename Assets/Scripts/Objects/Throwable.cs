using UnityEngine;
using System.Collections.Generic;

namespace Objects
{

    public class Throwable : MonoBehaviour
    {
        public float ThrowSpeed = 100f;
        public float Lifetime = 5f;
        public float RotateSpeed = 20f;

        public Vector3 ThrowDirection;
        public Transform PivotPoint;
        public List<MeshRenderer> m_MeshRenderers = new List<MeshRenderer>();

        private BoxCollider m_BoxCollider;
        private Rigidbody m_Rigidbody;        

        void OnEnable()
        {
            //Gather/set any variables
            m_Rigidbody = GetComponent<Rigidbody>();
            m_BoxCollider = GetComponent<BoxCollider>();
            //ThrowDirection = transform.forward;
            
            //This is for placeholder axe, will need to change for final axe, most likely
            foreach (MeshRenderer MR in m_MeshRenderers)
                MR.enabled = true;
            m_BoxCollider.enabled = true;
            m_Rigidbody.AddForce((ThrowDirection * ThrowSpeed), ForceMode.Impulse);
        }

        // Update is called once per frame
        void Update()
        { 
            if (Lifetime <= 0)
                Destroy(gameObject);
            else
                Lifetime -= Time.deltaTime;

            transform.RotateAround(PivotPoint.position, transform.right, RotateSpeed * Time.deltaTime);
        }
    }
}
