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
            if (PickupLevel == 2)
                GetComponent<Throwable>().damage = 2;
            if (PickupLevel == 3)
                GetComponent<Throwable>().damage = 3;
        }

        protected override void Effect(GameObject p_Driver)
        {
            // Throwable must be enabled before parent is null
            // Throwable is using parents velocity in OnEnable
 
            //Sets the throwable position above the center kart (temporary, should go from charcter position
            Transform l_SpawnLocation = p_Driver.transform.GetChild(0).Find("Raycast Points/Kart Front");
            transform.position = l_SpawnLocation.position;
            //transform.position = new Vector3(l_SpawnLocation.position.x, l_SpawnLocation.position.y + 0.5f, l_SpawnLocation.position.z);
            //transform.rotation = Quaternion.Euler(0, l_SpawnLocation.rotation.eulerAngles.y, 0);
            
            GetComponent<Throwable>().ThrowDirection = l_SpawnLocation.transform.forward;
            transform.rotation = l_SpawnLocation.transform.rotation;
            Vector3 l_SpawnRotation = transform.localEulerAngles;
            l_SpawnRotation.z -= 45;
            transform.localEulerAngles = l_SpawnRotation;
            GetComponent<Throwable>().enabled = true;
            if (PickupLevel == 2)
            {
                Debug.Log("Level 1 tracking enabled");
                GetComponent<Throwable>().EnableTracking(1);
            }
            //else if (PickupLevel == 3)
            //{
            //    Debug.Log("Level 2 tracking enabled");
            //    GetComponent<Throwable>().EnableTracking(2);
            //    GetComponent<Throwable>().AssignTrack(p_Driver.GetComponent<Driver>().CurrentSpline);
            //}
            transform.parent = null;

            base.Effect(p_Driver);
        }

        public override void DeletePickup(GameObject p_Driver)
        {
            base.DeletePickup(p_Driver);
        }
    }
}
