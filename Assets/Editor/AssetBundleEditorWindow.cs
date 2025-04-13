using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AssetBundleEditorWindow : EditorWindow
    {
        private string _directoryPath = "Assets/";
        private string _bundleName;

        private void OnGUI()
        {
            _directoryPath = EditorGUILayout.TextField("Directory Path: ", _directoryPath);

            EditorGUILayout.Space();
            _bundleName = EditorGUILayout.TextField("Bundle Name: ", _bundleName);

            EditorGUILayout.Space();
            if (GUILayout.Button("Set name for AssetBundles in directory"))
            {
                AssetBundleEditor.SetAssetBundleNames(_directoryPath, _bundleName);
            }
        }
    }
}
