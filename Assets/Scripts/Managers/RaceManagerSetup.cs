using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using  Managers;

public class RaceManagerSetup : MonoBehaviour
{
    public List<Transform> StartPositions;

    // Use this for initialization
    void Start()
    {
        //Managers.Racemanager.s_StartPositions = StartPositions;
       // Objects.Player l_NewPlayer = Instantiate(Objects.Player,) as GameObject;
        RaceManager.AddPlayer("Beaver Kart 1", "JS1");
        RaceManager.AddPlayer("Beaver Kart 1", "JS2");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
