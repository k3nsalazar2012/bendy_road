using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSystem : MonoBehaviour
{
    [SerializeField] GameObject roadPrefab;
    [SerializeField] int length;
    [SerializeField] private Transform waypointsTransform;

    private RoadPath currentRoad;
    List<RoadPath> roads;
    private List<Vector3> waypoints;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        GenerateRoadSet();
        //CreateNewTunnels();
    }

    private void GenerateRoadSet()
    {
        for (int i = 0; i < length; i++)
        {
            GameObject roadObject = Instantiate(roadPrefab);
            roadObject.transform.SetParent(transform);
            roadObject.transform.localScale = Vector3.one;
            roadObject.name = "Road";

            if (currentRoad == null)
            {
                roadObject.transform.localPosition = Vector3.zero;
                roadObject.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                roadObject.transform.localPosition = currentRoad.EdgePosition;
                roadObject.transform.localEulerAngles = currentRoad.EdgeRotation;
            }

            currentRoad = roadObject.GetComponent<RoadPath>();
            currentRoad.Generate();

            GenerateWaypoints();
        }
    }

    private void GenerateWaypoints()
    {
        if (waypoints == null) waypoints = new List<Vector3>();
        if (player.Waypoints == null) player.Waypoints = new List<Vector3>();

        foreach (var point in currentRoad.Waypoints)
        {
            waypoints.Add(point);
            player.Waypoints.Add(point);

            GameObject newPoint = new GameObject();
            newPoint.transform.SetParent(waypointsTransform);
            newPoint.transform.localPosition = point;
            newPoint.transform.localScale = Vector3.one;
            newPoint.name = "Waypoint";
        }

        player.Move = true;
    }
}
