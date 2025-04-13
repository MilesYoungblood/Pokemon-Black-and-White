using UnityEditor;
using UnityEngine;

namespace Editor
{
    public abstract class AssetBundleEditor
    {
        [MenuItem("Tools/AssetBundle Editor/Refactor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<AssetBundleEditorWindow>("AssetBundle Editor");
        }

        public static void SetAssetBundleNames(string path, string bundleName)
        {
            foreach (var guid in AssetDatabase.FindAssets("", new[] { path }))
            {
                var assetImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                if (assetImporter)
                {
                    assetImporter.assetBundleName = bundleName;
                }
            }

            MonoBehaviour.print("AssetBundle names set.");
        }
    }
}
