using UnityEngine;
using System.Collections;

namespace Objects
{
    public class MinePickup : Pickup
    {

        public override void LevelUp(GameObject p_Driver)
        {
            base.LevelUp(p_Driver);
            //Level 2 damage boost
            if (PickupLevel == 2)
            {
                PickupDamage = 2;
                GetComponent<Mine>().damage = PickupDamage;
            }
            if (PickupLevel == 3)
            {
                GetComponent<SphereCollider>().radius = 1;
                GetComponent<Mine>().radius = 3.5f;
                PickupUses = 2;
                PickupDamage = 3;
                GetComponent<Mine>().damage = PickupDamage;
            }
        }

        protected override void Effect(GameObject p_Driver)
        {
            // TODO level 3 mine isn't allowing multiple uses, LevelUp function is okay
            // Should only be called after level 3 (since that grants extra use)
            if (PickupUses > 1)
            {
                base.Effect(p_Driver);
                p_Driver.GetComponent<Driver>().CurrentPickup = Instantiate(gameObject);
            }

            Transform l_SpawnLocation = p_Driver.transform.GetChild(0);

            transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y - 0.5f, l_SpawnLocation.position.z);
            GetComponent<Mine>().enabled = true;
            transform.parent = null;
            base.Effect(p_Driver);
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
        }
    }
}