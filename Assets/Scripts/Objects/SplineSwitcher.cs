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
            if (p_OtherCollider.name == "DriverTriggerCollider" || p_OtherCollider.name.Equals("Throwing Axe"))
            {
                Controllers.AIController l_AIController = p_OtherCollider.transform.root.gameObject.GetComponent<Controllers.AIController>();
                Throwable l_Axe = p_OtherCollider.transform.root.gameObject.GetComponent<Throwable>();

                if (l_AIController != null)
                    l_AIController.SwitchSpline(SplineOptions);
                else if (l_Axe != null)
                    l_Axe.SwitchSpline(SplineOptions);
            }
        }
    }
}