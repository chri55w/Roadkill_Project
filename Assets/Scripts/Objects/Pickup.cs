using UnityEngine;
using System.Collections;

namespace Objects
{
    public class Pickup : MonoBehaviour
    {
        public int PickupLevel = 1;
        public string PickupName;
        //Pickup Image for GUI

        public void UsePickup(GameObject p_Driver)
        {
            switch (PickupLevel)
            {
                case 1:
                    LevelOneEffect(p_Driver);
                    break;
                case 2:
                    LevelTwoEffect(p_Driver);
                    break;
                case 3:
                    LevelThreeEffect(p_Driver);
                    break;
            }
        }

        protected virtual void LevelOneEffect(GameObject p_Driver)
        {
            Debug.Log("Do something");
        }

        protected virtual void LevelTwoEffect(GameObject p_Driver)
        {
            Debug.Log("Do something cool");
        }

        protected virtual void LevelThreeEffect(GameObject p_Driver)
        {
            Debug.Log("Do something awesome");
        }
    }
}
