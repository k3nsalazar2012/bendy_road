using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadPath : MonoBehaviour
{
    [SerializeField] private int length;

    [Header("Mesh Bending")]
    [SerializeField] private BendAxis axis;
    [SerializeField] private int rotate;
  
    private const int width = 2;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Vector3[] waypoints;

    // private variables for mesh bending
    private const float fromPosition = 0.5f;

    public enum BendAxis { X, Y, Z};

    void Start()
    {
        axis = (BendAxis) Random.Range(0, System.Enum.GetValues(typeof(BendAxis)).Length-1);
        rotate = Random.Range(-90, 90);
        //print(axis + " | " + rotate);
        //Generate();
    }

    public void Generate()
    {
        axis = (BendAxis)Random.Range(0, System.Enum.GetValues(typeof(BendAxis)).Length - 1);
        rotate = Random.Range(-20, 20);

        mesh = GetComponent<MeshFilter>().mesh = new Mesh();
        mesh.Clear();

        SetVertices();
        SetUVs();
        SetTriangles();
        UpdateMesh();

        BendMesh();
        print(axis + " | " + rotate);
    }

    private void SetVertices()
    {
        vertices = new Vector3[(width + 1) * (length + 1)];
        Vector3 pos = Vector3.zero;

        for (int z = 0, i = 0; z <= length; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                pos.x = x - (width/2);
                pos.y = 0f;
                pos.z = z;
                vertices[i] = pos;
                i++;
            }
        }
    }

    private void SetUVs()
    {
        uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
    }

    private void SetTriangles()
    {
        triangles = new int[width * length * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void BendMesh()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;

        if (axis == BendAxis.X)
        {
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
        }
        else if (axis == BendAxis.Y)
        {
            float meshWidth = mesh.bounds.size.z;
            for (var i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].z + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }
        else if (axis == BendAxis.Z)
        {
            float meshWidth = mesh.bounds.size.x;
            for (var i = 0; i < vertices.Length; i++)
            {
                float formPos = Mathf.Lerp(meshWidth / 2, -meshWidth / 2, fromPosition);
                float zeroPos = vertices[i].x + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / meshWidth);

                zeroPos -= 2 * vertices[i].y * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                vertices[i].y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                vertices[i].x = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;
            }
        }

        UpdateMesh();

        waypoints = new Vector3[length + 1];

        for (int i = 0; i <= length; i++)
        {
            float formPos = Mathf.Lerp(length / 2, -length / 2, fromPosition);
            float zeroPos = i + formPos;
            float rotateValue = (-rotate / 2) * (zeroPos / length);
            zeroPos -= 2 * 0f * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

            float x, y, z;
            Vector3 point;

            switch (axis)
            {
                case BendAxis.X:
                    x = transform.localPosition.x;
                    z = i;

                    x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                    z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;

                    point = new Vector3(x, transform.localPosition.y, z + transform.localPosition.z);
                    waypoints[i] = RotatePointAroundPivot(point, transform.localPosition, transform.localEulerAngles.y);

                    if (i == length)
                    {
                        EdgePosition = waypoints[i];
                        EdgeRotation = transform.localEulerAngles - (Vector3.up * rotate);
                    }
                    break;
                case BendAxis.Y:
                    y = transform.localPosition.y;
                    z = i;
                    y += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                    z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;

                    point = new Vector3(transform.localPosition.x, y, z + transform.localPosition.z);
                    waypoints[i] = RotatePointAroundPivot(point, transform.localPosition, transform.localEulerAngles.x);

                    if (i == length)
                    {
                        EdgePosition = waypoints[i];
                        EdgeRotation = transform.localEulerAngles - (Vector3.left * rotate);
                    }
                    break;
                case BendAxis.Z: break;
            }
                    
        }
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angles)
    {
        var dir = point - pivot;
        return transform.TransformPoint(dir);
    }

    public Vector3 EdgePosition { set; get; }
    public Vector3 EdgeRotation { set; get; }
    public Vector3[] Waypoints
    {
        get { return waypoints; }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;

        foreach (var point in waypoints)
        {
            Gizmos.DrawWireSphere(point, 0.25f);
        }
    }
}
