using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSystem : MonoBehaviour
{
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform waypointsTransform;

    private List<Road> roads;
    private List<Vector3> waypoints;
    private Road currentRoad;
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        for (int i = 0; i < 10; i++)
            GenerateTunnelSet();
        //CreateNewTunnels();
    }

    public void CreateNewTunnels()
    {
        ClearPreviousTunnels();

        for (int i = 0; i < 2; i++)
            GenerateTunnelSet();
    }

    private void ClearPreviousTunnels()
    {
        if (transform.childCount == 0) return;

        for (int i = 0; i < 2; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void GenerateTunnelSet()
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

        currentRoad = roadObject.GetComponent<Road>();
        currentRoad.Generate();

        GenerateWaypoints();
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
