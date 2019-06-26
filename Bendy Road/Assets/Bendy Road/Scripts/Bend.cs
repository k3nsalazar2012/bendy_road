using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bend : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;

    private const float fromPosition = 0.5f;
    private int rotate;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;

        rotate = 90;

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

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
