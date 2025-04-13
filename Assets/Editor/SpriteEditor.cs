using System;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using Object = UnityEngine.Object;

namespace Editor
{
    public class SpriteEditor : AssetPostprocessor
    {
        // Editor window to manually rename sprites
        [MenuItem("Tools/Sprite Editor/Refactor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<SpriteEditorWindow>("Sprite Editor");
        }

        // Method to rename the base asset
        public static void RenameBaseAsset(string assetPath, string suffix)
        {
            var directory = System.IO.Path.GetDirectoryName(assetPath);
            var filename = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            var extension = System.IO.Path.GetExtension(assetPath);

            var newFilename = filename + suffix;
            var newPath = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(directory ?? throw new InvalidOperationException(), newFilename + extension));

            AssetDatabase.RenameAsset(assetPath, newFilename + extension);
            AssetDatabase.ImportAsset(newPath, ImportAssetOptions.ForceUpdate);
        }

        // Method to rename sprites using ISpriteEditorDataProvider
        public static void RenameSprites(TextureImporter textureImporter, string name)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            if (dataProvider == null)
            {
                return;
            }

            dataProvider.InitSpriteEditorDataProvider();
            var spriteRects = dataProvider.GetSpriteRects();
            foreach (var rect in spriteRects)
            {
                var underscoreIndex = rect.name.LastIndexOf('_');
                rect.name = underscoreIndex > 0 ? $"{name}{rect.name[underscoreIndex..]}" : name;
            }

            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();
        }

        // Method to clean sprite names using ISpriteEditorDataProvider
        public static void ReplaceSpriteNames(Object textureImporter, string oldValue, string newValue)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            if (dataProvider == null)
            {
                return;
            }

            dataProvider.InitSpriteEditorDataProvider();
            var spriteRects = dataProvider.GetSpriteRects();
            foreach (var rect in spriteRects)
            {
                if (rect.name.Contains(oldValue))
                {
                    rect.name = rect.name.Replace(oldValue, newValue);
                }
            }

            dataProvider.SetSpriteRects(spriteRects);
            dataProvider.Apply();
        }
    }
}
