using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Road : MonoBehaviour
{
    [SerializeField] private int sides;
    [SerializeField] private float radius;
    [SerializeField] private int length;
    [SerializeField] private GameObject vertexPrefab;

    [Header("Mesh Bending")]
    [SerializeField] private int minRotate;
    [SerializeField] private int maxRotate;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private List<Vector3> points;
    private Vector3[] waypoints;

    // private variables for mesh bending
    private const float fromPosition = 0.5f;
    private int rotate;

    public void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();

        SetVertices();
        SetTriangles();
        UpdateMesh();

        BendMesh();
    }

    private void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        Debug.Log("vertices: " + vertices.Length);
        Debug.Log("triangles: " + triangles.Length);

        /*foreach (var vert in vertices)
        {
            GameObject vertex = Instantiate(vertexPrefab, transform, true);
            vertex.transform.position = vert;
            vertex.name = (transform.childCount - 1) + " : " + vert;
        }

        foreach (var tri in triangles)
        {
            //Debug.Log("tri: " + tri);
        }*/
    }

    private void SetVertices()
    {
        points = new List<Vector3>();

        vertices = new Vector3[(sides + 1) * (length + 1)];
        float angle = 2f * Mathf.PI / sides;
        Vector3 pos = Vector3.zero;

        for (int z = 0, i = 0; z <= length; z++)
        {
            for (int x = 0; x <= sides; x++)
            {
                pos.x = radius * Mathf.Cos(x * angle);
                pos.y = radius * Mathf.Sin(x * angle);
                pos.z = z;
                vertices[i] = pos;
                points.Add(pos);
                i++;
            }
        }
    }

    private void SetTriangles()
    {
        triangles = new int[sides * length * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < sides; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + sides + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + sides + 1;
                triangles[tris + 5] = vert + sides + 2;
                
                vert++;
                tris += 6;
            }
            vert++;
        }        
    }

    private void BendMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        rotate = (int)UnityEngine.Random.Range(minRotate, maxRotate);

        float meshWidth = mesh.bounds.size.z;
        for (var i = 0; i < vertices.Length; i++)
        {
            float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
            float zeroPos = vertices[i].z + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

            zeroPos -= 2 * vertices[i].x * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            vertices[i].x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
            vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
        }

        UpdateMesh();

        waypoints = new Vector3[length + 1];

        for (int i = 0; i <= length; i++)
        {
            float formPos = Mathf.Lerp(length / 2, -length / 2, fromPosition);
            float zeroPos = i + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / length);
            zeroPos -= 2 * 0f * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            float x = transform.localPosition.x;
            float z = i;
            x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
            z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;

            Vector3 point = new Vector3(x, 0f, z + transform.localPosition.z);
            waypoints[i] = RotatePointAroundPivot(point, transform.localPosition, transform.localEulerAngles.y);
        }
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angles)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(Vector3.up * angles) * dir;
        point = dir + pivot;
        return point;
    }

    public Vector3 EdgePosition
    {
        get { return waypoints[waypoints.Length - 1]; }
    }
    public Vector3 EdgeRotation
    {
        get { return transform.localEulerAngles - (Vector3.up * rotate); }
    }
    public Vector3[] Waypoints
    {
        get { return waypoints; }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;

        foreach (Vector3 point in waypoints)
        {
            Gizmos.DrawSphere(point, 0.25f);
        }
    }
}
