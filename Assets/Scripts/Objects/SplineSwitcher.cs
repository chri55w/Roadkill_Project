using UnityEngine;
using System.Collections.Generic;

namespace Objects
{
    public class SplineSwitcher : MonoBehaviour
    {
        [SerializeField]
        public List<BezierSpline> SplineOptions;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider p_OtherCollider)
        {

            if (p_OtherCollider.name == "DriverTriggerCollider")
            {
                Controllers.AIController l_AIController = p_OtherCollider.transform.root.gameObject.GetComponent<Controllers.AIController>();

                if (l_AIController != null)
                    l_AIController.SwitchSpline(SplineOptions);
            }
        }
    }
}