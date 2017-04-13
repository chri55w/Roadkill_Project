using UnityEngine;
using System.Collections;

namespace Objects
{
    public class SpeedBoost : Pickup
    {
        public float BoostForce;

        protected override void Effect(GameObject p_Driver)
        {
            p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);            
            base.Effect(p_Driver);
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
            Destroy(gameObject);
        }
    }
}