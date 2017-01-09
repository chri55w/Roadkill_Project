using UnityEngine;
using System.Collections;

namespace Objects
{

    public class SplineSetter : MonoBehaviour
    {
        [SerializeField]
        public BezierSpline Spline;

        void OnTriggerEnter(Collider p_OtherCollider)
        {
            if (p_OtherCollider.name == "DriverTriggerCollider")
            {
                Driver l_Driver = p_OtherCollider.transform.root.gameObject.GetComponent<Driver>();

                if (l_Driver != null)
                    l_Driver.ChangeSpline(Spline);
            }
        }
    }
}