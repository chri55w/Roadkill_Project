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

        //Old code: Separate Level Functions
        //protected override void LevelOneEffect(GameObject p_Driver)
        //{
        //    base.LevelOneEffect(p_Driver);
        //    p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        //}
        //protected override void LevelTwoEffect(GameObject p_Driver)
        //{
        //    base.LevelTwoEffect(p_Driver);
        //    p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        //}
        //protected override void LevelThreeEffect(GameObject p_Driver)
        //{
        //    base.LevelThreeEffect(p_Driver);
        //    p_Driver.GetComponentInChildren<Controllers.KartController>().Boost(BoostForce * PickupLevel);
        //}
    }
}