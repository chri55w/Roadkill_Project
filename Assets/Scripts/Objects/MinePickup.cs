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
            if (PickupLevel == 3)
            {
                GetComponent<SphereCollider>().radius = 1;
                GetComponent<Mine>().radius = 3.5f;
                PickupUses = 2;
            }
        }

        protected override void Effect(GameObject p_Driver)
        {
            // Should only be called after level 3 (since that grants extra use), possibly need a check in
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

        //Old Code: Separate Level Functions
        //protected override void LevelOneEffect(GameObject p_Driver)
        //{
        //    base.LevelOneEffect(p_Driver);
        //    Transform l_SpawnLocation = p_Driver.transform.GetChild(0);
           
        //    transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y - 0.5f, l_SpawnLocation.position.z);
        //    GetComponent<Mine>().enabled = true;
        //    transform.parent = null;
        //}

        //protected override void LevelTwoEffect(GameObject p_Driver)
        //{
        //    base.LevelTwoEffect(p_Driver);
        //    Transform l_SpawnLocation = p_Driver.transform.GetChild(0);
            
        //    transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y - 0.5f, l_SpawnLocation.position.z);
        //    GetComponent<Mine>().enabled = true;
        //    transform.parent = null;
        //}

        //protected override void LevelThreeEffect(GameObject p_Driver)
        //{
        //    //Reduce Pickup Use at start

        //    //When level 3 is activated, if there are any uses left create duplicate a new mine for the driver to replace the current one 
        //    if (PickupUses > 1)
        //    {
        //        base.LevelThreeEffect(p_Driver);
        //        p_Driver.GetComponent<Driver>().CurrentPickup = Instantiate(gameObject);
        //    }

        //    base.LevelThreeEffect(p_Driver);
        //    //Add check to make sure this is the kart
        //    Transform l_SpawnLocation = p_Driver.transform.GetChild(0);
            
        //    transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y - 0.5f, l_SpawnLocation.position.z);
        //    GetComponent<SphereCollider>().radius = 1;
        //    GetComponent<Mine>().radius = 3.5f;
        //    GetComponent<Mine>().enabled = true;
        //    transform.parent = null;
        //}

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
        }
    }
}