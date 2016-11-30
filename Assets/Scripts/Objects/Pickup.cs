using UnityEngine;
using System.Collections;

namespace Objects
{
    public class Pickup : MonoBehaviour
    {
        public int PickupLevel = 1;
        public int PickupUses = 1;
        public string PickupName;
        //Pickup Image for GUI

        public void UsePickup(GameObject p_Driver)
        {
            Effect(p_Driver);
           
            //switch (PickupLevel)
            //{
            //    case 1:
            //        LevelOneEffect(p_Driver);
            //        break;
            //    case 2:
            //        LevelTwoEffect(p_Driver);
            //        break;
            //    case 3:
            //        LevelThreeEffect(p_Driver);
            //        break;
            //}
           
        }

        public virtual void LevelUp(GameObject p_Driver)
        {
            PickupLevel++;
        }

        protected virtual void Effect(GameObject p_Driver)
        {
            PickupUses--;
        }

        public virtual void DeletePickup(GameObject p_Driver)
        {
            transform.parent = null;
            p_Driver.GetComponent<Driver>().CurrentPickup = null;
            Destroy(this);
        }

        //protected virtual void LevelOneEffect(GameObject p_Driver)
        //{
        //    PickupUses--;
        //}

        //protected virtual void LevelTwoEffect(GameObject p_Driver)
        //{
        //    PickupUses--;
        //}

        //protected virtual void LevelThreeEffect(GameObject p_Driver)
        //{
        //    PickupUses--;
        //}
    }
}
