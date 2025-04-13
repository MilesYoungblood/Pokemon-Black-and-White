using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Editor
{
    public class TextureImportSettings : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            var textureImporter = (TextureImporter)assetImporter;

            if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(assetImporter);
                if (dataProvider != null)
                {
                    dataProvider.InitSpriteEditorDataProvider();
                    dataProvider.Apply();
                }
            }

            // Set the compression quality
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

            // Set the filter mode
            textureImporter.filterMode = FilterMode.Point;

            // Apply changes
            textureImporter.SaveAndReimport();
        }
    }
}
