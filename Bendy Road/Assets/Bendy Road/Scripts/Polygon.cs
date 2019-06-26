using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Polygon : MonoBehaviour
{
    [SerializeField] private int sides;
    [SerializeField] private float radius;
    [SerializeField] private int length;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    List<Vector3> points;
    void Start()
    {
        /*points = new List<Vector3>();

        float angle = 2f * Mathf.PI / sides;
        for (int i = 0; i < sides; i++)
        {
            float x = radius * Mathf.Sin(i * angle);
            float y = radius * Mathf.Cos(i * angle);
            points.Add(new Vector3(x, y, 0));
        }*/
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.Clear();

        SetVertices();
        SetTriangles();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void SetVertices()
    {
        vertices = new Vector3[(sides + 1) * (length + 1)];
        float angle = 2f * Mathf.PI / sides;
        Vector3 pos = Vector3.zero;

        for (int z = 0, i = 0; z <= length; z++)
        {
            for (int x = 0; x <= sides; x++)
            {
                pos.x = radius * Mathf.Cos(i * angle);
                pos.y = radius * Mathf.Sin(i * angle);
                pos.z = z;
                vertices[i] = pos;
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.25f);

        if (points != null)
        {
            foreach (Vector3 point in points)
            {
                Gizmos.DrawSphere(point, 0.25f);
            }
        }
    }
}
