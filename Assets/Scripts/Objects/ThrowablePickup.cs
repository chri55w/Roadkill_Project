using UnityEngine;
using System.Collections;

namespace Objects
{
    public class ThrowablePickup : Pickup
    {

        public override void LevelUp(GameObject p_Driver)
        {
            base.LevelUp(p_Driver);
            //Add/Allow tracking and damage increase
        }

        protected override void Effect(GameObject p_Driver)
        {
            Debug.Log("Thrown");
            //Sets the throwable position above the center kart (temporary, should go from charcter position
            Transform l_SpawnLocation = p_Driver.transform.GetChild(0);
            transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y + 1.0f, l_SpawnLocation.position.z);
            //transform.rotation = Quaternion.Euler(0, l_SpawnLocation.rotation.eulerAngles.y, 0);
            transform.parent = null;
            GetComponent<Throwable>().ThrowDirection = l_SpawnLocation.transform.forward;
            transform.rotation = l_SpawnLocation.transform.rotation;
            Vector3 l_SpawnRotation = transform.localEulerAngles;
            l_SpawnRotation.z -= 45;
            transform.localEulerAngles = l_SpawnRotation;
            
            GetComponent<Throwable>().enabled = true;

            base.Effect(p_Driver);
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
        }
    }
}
