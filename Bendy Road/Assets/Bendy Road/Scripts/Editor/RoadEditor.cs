using UnityEngine;
using UnityEditor;

namespace BendyRoad
{
    [CustomEditor(typeof(Road))]
    public class RoadEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Road road = (Road)target;

            if(GUILayout.Button("Generate Road"))
            {
                road.GenerateMesh();
            }
        }
    }
}