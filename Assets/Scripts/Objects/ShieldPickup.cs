using UnityEngine;

namespace Objects
{
    public class ShieldPickup : Pickup
    {
        public int duration = 3;
        public override void LevelUp(GameObject p_Driver)
        {
            base.LevelUp(p_Driver);
            if (PickupLevel == 2)
                duration = 5;
            else if (PickupLevel == 3)
                duration = 8;
        }

        protected override void Effect(GameObject p_Driver)
        {
            p_Driver.GetComponentInChildren<Shield>().Driver = p_Driver;
            p_Driver.GetComponentInChildren<Shield>().lifetime = duration;
            p_Driver.GetComponentInChildren<Shield>().enabled = true;
            base.Effect(p_Driver);            
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            transform.parent = null;
            p_Driver.GetComponent<Driver>().CurrentPickup = null;
            Destroy(gameObject);
        }
    }
}