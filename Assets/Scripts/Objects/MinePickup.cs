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
            // TODO add rigidbody or gravity so mine is on floor
            if (PickupUses > 1)
            {
                base.Effect(p_Driver);
                GameObject l_MineCopy = Instantiate(gameObject);
                l_MineCopy.transform.SetParent(p_Driver.transform);
                p_Driver.GetComponent<Driver>().CurrentPickup = l_MineCopy;
            }
            else
            {
                base.Effect(p_Driver);
            }

            transform.parent = null;

            Transform l_SpawnLocation = p_Driver.transform.GetChild(0);

            transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y - 0.5f, l_SpawnLocation.position.z);
            GetComponent<Mine>().enabled = true;     
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
        }
    }
}