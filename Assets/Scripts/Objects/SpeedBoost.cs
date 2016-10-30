using UnityEngine;
using System.Collections;

namespace Objects
{
    public class SpeedBoost : Pickup
    {

        public float BoostForce;

        protected override void LevelOneEffect(GameObject p_Driver)
        {
            p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        }
        protected override void LevelTwoEffect(GameObject p_Driver)
        {
            p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        }
        protected override void LevelThreeEffect(GameObject p_Driver)
        {
            p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        }
    }
}