using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Scripts.Source.Editor
{
    [CustomEditor(typeof(Cluster))]
    public class ClusterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
            {
                ((Cluster)target).Generate();
            }
            else if (GUILayout.Button("Clear"))
            {
                ((Cluster)target).Clear();
            }
        }
    }
}
