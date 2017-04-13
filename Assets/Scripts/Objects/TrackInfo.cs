using UnityEngine;
using System.Collections.Generic;

using Objects;

using Controllers;

public class TrackInfo : MonoBehaviour {

    public string TrackName;
    public List<WaypointController> Waypoints;
    public WaypointController StartFinishWaypoint;
    public List<Transform> StartPositions;
    public BezierSpline StartingSpline;
    public Sprite MiniMapMaterial;
    public Vector2 MapSize;
    public Vector2 MiniMapCharacterOffset;
    public BezierSpline LapSpline;

    public BezierSpline OpeningCinematicSpline;
    public GameObject OpeningCinematicTarget;

    public BezierSpline PodiumCinematicSpline;
    public GameObject PodiumCinematicTarget;

    public List<GameObject> PodiumKartPositions;
    public List<GameObject> PodiumCharacterPositions;
    public GameObject PodiumAssets;

    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public bool ConfirmLap(List<WaypointController> p_Waypoints)
    {
        if (p_Waypoints.Count <= 0)
            return false;

        if (!p_Waypoints[p_Waypoints.Count - 1].NextWaypoints.Contains(p_Waypoints[0]))
            return false;

        for (int i = 0; i < p_Waypoints.Count-1; i++)
        {
            if (!p_Waypoints[i].NextWaypoints.Contains(p_Waypoints[i+1]))
            {
                return false;
            }
        }

        return true;
    }
}
