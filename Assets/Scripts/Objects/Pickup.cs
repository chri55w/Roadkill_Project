using UnityEngine;
using System.Collections;

namespace Objects
{
    public class Pickup : MonoBehaviour
    {
        public int PickupLevel = 1;
        public int PickupUses = 1;
        public int PickupDamage = 0;

        public enum e_PickupID { SPEED_BOOST,  LAND_MINE, THROWING_AXE, SHIELD, BLOOD_SLICK };
        public e_PickupID PickupID;
        //Pickup Image for GUI
        [SerializeField]
        protected Texture2D Level1Icon;
        [SerializeField]
        protected Texture2D Level2Icon;
        [SerializeField]
        protected Texture2D Level3Icon;

        public void UsePickup(GameObject p_Driver)
        {
            Effect(p_Driver);           
        }

        public virtual void LevelUp(GameObject p_Driver)
        {
            PickupLevel++;
        }

        protected virtual void Effect(GameObject p_Driver)
        {
            PickupUses--;
        }

        public Texture2D GetCurrentIcon()
        {
            switch(PickupLevel)
            {
                case 1:
                    return Level1Icon;
                case 2:
                    return Level2Icon;
                case 3:
                    return Level3Icon;
                default:
                    return Level1Icon;
            }
        }

        public virtual void DeletePickup(GameObject p_Driver)
        {
            transform.parent = null;
            p_Driver.GetComponent<Driver>().CurrentPickup = null;
            Destroy(this);
        }
    }
}
