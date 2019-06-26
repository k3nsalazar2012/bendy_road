using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BendyRoad
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Road : MonoBehaviour
    {
        [Header("Mesh Generation")]
        [SerializeField] private int xSize;
        [SerializeField] private int ySize;
        [SerializeField] private int zSize;

        [Header("Mesh Bending")]
        [SerializeField] private int bendAngle;

        // private variables for mesh generation
        private const float thetaScale = 0.1f;
        private Mesh mesh;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector3[] normals;

        // private variables for mesh bending
        private const float fromPosition = 0.5f;
        private int rotate;
        private Vector3[] waypoints;

        public void GenerateMesh()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.Clear();

            int cornerVertices = 8; // cube has 8 corner virtices
            int edgeVertices = (xSize + ySize + zSize - 3) * 4;
            int faceVertices = (
                (xSize - 1) * (ySize - 1) +
                (xSize - 1) * (zSize - 1) +
                (ySize - 1) * (zSize - 1)) * 2;
            vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
            normals = new Vector3[vertices.Length];

            SetVertices();
            SetTriangles();
            mesh.RecalculateNormals();

            if (bendAngle != 0)
            {
                Bend();
            }

            GenerateWaypoints();
        }

        #region Setting Vertices of the Mesh
        private void SetVertices()
        {
            int v = 0;
            Vector3 offset = new Vector3((float)xSize / 2f,(float) ySize / 2f, 0f);
            for (int y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                    SetVertex(v++, x, y, 0); // vertices[v++] = new Vector3(x, y, 0) - offset;

                for (int z = 1; z <= zSize; z++)
                    SetVertex(v++, xSize, y, z); //vertices[v++] = new Vector3(xSize, y, z) - offset;

                for (int x = xSize - 1; x >= 0; x--)
                    SetVertex(v++, x, y, zSize); // vertices[v++] = new Vector3(x, y, zSize) - offset;

                for (int z = zSize - 1; z > 0; z--)
                    SetVertex(v++, 0, y, z); // vertices[v++] = new Vector3(0, y, z) - offset;
            }

            for (int z = 1; z < zSize; z++)
            {
                for (int x = 1; x < xSize; x++)
                    SetVertex(v++, x, ySize, z); // vertices[v++] = new Vector3(x, ySize, z) - offset;
            }

            for (int z = 1; z < zSize; z++)
            {
                for (int x = 1; x < xSize; x++)
                    SetVertex(v++, x, 0, z); //vertices[v++] = new Vector3(x, 0, z) - offset;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
        }
        #endregion

        #region Setting Triangles of the Mesh
        private void SetTriangles()
        {
            int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
            triangles = new int[quads * 6];

            int ring = (xSize + zSize) * 2;
            int t = 0, v = 0;

            for (int y = 0; y < ySize; y++, v++)
            {
                for (int q = 0; q < ring - 1; q++, v++)
                {
                    t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                }
                t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            }
            t = CreateTopFace(triangles, t, ring);
            t = CreateBottomFace(triangles, t, ring);
            mesh.triangles = triangles;
        }

        private int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * ySize;
            for (int x = 0; x < xSize - 1; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = ring * (ySize + 1) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);

            t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
            return t;
        }
        private int CreateBottomFace(int[] triangles, int t, int ring)
        {
            int v = 1;
            int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (int x = 1; x < xSize - 1; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

            int vMin = ring - 2;
            vMid -= xSize - 2;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(
                        triangles, t,
                        vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
                }
                t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
            }

            int vTop = vMin - 1;
            t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
            return t;
        }
        private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
        {
            triangles[i] = v00;
            triangles[i + 1] = triangles[i + 4] = v01;
            triangles[i + 2] = triangles[i + 3] = v10;
            triangles[i + 5] = v11;
            return i + 6;
        }
        #endregion

        #region Bending the Mesh
        private void Bend()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            rotate = bendAngle;

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
            mesh.normals = normals;
            //mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
        #endregion

        #region Generating Waypoints
        private void GenerateWaypoints()
        {
            waypoints = new Vector3[zSize+1];

            for (int i = 0; i <= zSize; i++)
            {
                float formPos = Mathf.Lerp((float) zSize / 2, (float) -zSize / 2, fromPosition);
                float zeroPos = i + formPos;
                float rotateValue = (-rotate / 2) * (zeroPos / zSize);
                zeroPos -= 2 * 0f * Mathf.Cos((90 - rotateValue) * Mathf.Deg2Rad);

                float x = transform.localPosition.x;
                float z = i;
                x += zeroPos * Mathf.Sin(rotateValue * Mathf.Deg2Rad);
                z = zeroPos * Mathf.Cos(rotateValue * Mathf.Deg2Rad) - formPos;

                Vector3 point = new Vector3(x, (float) ySize/2, z + transform.localPosition.z);
                waypoints[i] = RotatePointAroundPivot(point, transform.localPosition, transform.localEulerAngles.y);
            }

            Debug.Log(EdgePosition);
            Debug.Log(EdgeRotation);
            mesh.RecalculateNormals();
        }

        private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angles)
        {
            var dir = point - pivot;
            dir = Quaternion.Euler(Vector3.up * angles) * dir;
            point = dir + pivot;
            return point;
        }

        private void SetVertex(int i, float x, float y, float z)
        {
            Vector3 offset = new Vector3((float)xSize / 2f, (float)ySize / 2f, 0f);
            Vector3 inner = vertices[i] = new Vector3(x, y, z) - offset;

            float roundness = 0f;

            if (x < roundness)
            {
                inner.x = Random.Range(roundness - 0.2f, roundness + 0.2f);
            }
            else if (x > xSize - roundness)
            {
                inner.x = xSize - Random.Range(roundness - 0.2f, roundness + 0.2f);
            }


            if (y < roundness)
            {
                inner.y = Random.Range(roundness - 0.1f, roundness + 0.1f);
            }
            else if (y > ySize - roundness)
            {
                inner.y = ySize - Random.Range(roundness - 0.1f, roundness + 0.1f);
            }
            if (z < roundness)
            {
                inner.z = Random.Range(roundness - 0.2f, roundness + 0.2f);
            }
            else if (z > zSize - roundness)
            {
                inner.z = zSize - Random.Range(roundness - 0.2f, roundness + 0.2f);
            }

            normals[i] = (vertices[i] - inner).normalized;
            vertices[i] = inner + normals[i] * 0f;

        }
        #endregion

        public Vector3 EdgePosition
        {
            get
            {
                Vector3 position = waypoints[waypoints.Length - 1];
                position.y -= (float) ySize / 2;
                return position;
            }
        }

        public Vector3 EdgeRotation
        {
            get { return transform.localEulerAngles - (Vector3.up * rotate); }
        }

        public Vector3[] Waypoints
        {
            get { return waypoints; }
        }

        void OnDrawGizmos()
        {
            if (waypoints == null)
                return;

            foreach (var point in waypoints)
            {
                Gizmos.DrawSphere(point, 0.25f);
            }
        }
    }


}