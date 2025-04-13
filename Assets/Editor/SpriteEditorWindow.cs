using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SpriteEditorWindow : EditorWindow
    {
        private string _directoryPath = "Assets/Textures/Sprites/";
        private string _name;
        private bool _includeBaseSprite;
        private bool _includeSubSprites;
        private string _newName;

        private void OnGUI()
        {
            GUILayout.Label("Sprite Directory Path:", EditorStyles.boldLabel);
            _directoryPath = EditorGUILayout.TextField("Directory Path:", _directoryPath);

            EditorGUILayout.Space();
            _name = EditorGUILayout.TextField("Name:", _name);

            EditorGUILayout.Space();
            _newName = EditorGUILayout.TextField("New Name: ", _newName);

            EditorGUILayout.Space();
            _includeBaseSprite = EditorGUILayout.Toggle("Include main sprite?", _includeBaseSprite);

            EditorGUILayout.Space();
            _includeSubSprites = EditorGUILayout.Toggle("Include sub sprites?", _includeSubSprites);

            if (GUILayout.Button("Rename Sprites in Directory"))
            {
                RenameSpritesInDirectory();
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Replace Sprite Names in Directory"))
            {
                ReplaceSpriteNamesInDirectory();
            }
        }

        private void RenameSpritesInDirectory()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:texture2D", new[] { _directoryPath }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter)
                {
                    if (_includeBaseSprite)
                    {
                        SpriteEditor.RenameBaseAsset(assetPath, _name);
                    }

                    if (_includeSubSprites)
                    {
                        SpriteEditor.RenameSprites(textureImporter, _name);
                    }

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // Force re-import to see changes
                    MonoBehaviour.print($"Sprites renamed for {assetPath}");
                }
                else
                {
                    MonoBehaviour.print($"Failed to find or cast importer for {assetPath}");
                }
            }
        }

        private void ReplaceSpriteNamesInDirectory()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:texture2D", new[] { _directoryPath }))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter)
                {
                    SpriteEditor.ReplaceSpriteNames(textureImporter, _name, _newName);
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate); // Force re-import to see changes
                    MonoBehaviour.print($"Sprites replaced for {assetPath}");
                }
                else
                {
                    MonoBehaviour.print($"Failed to find or cast importer for {assetPath}");
                }
            }
        }
    }
}
