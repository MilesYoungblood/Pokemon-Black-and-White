using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public class TextureImportSettings : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            var textureImporter = (TextureImporter)assetImporter;

            if (textureImporter.spriteImportMode == SpriteImportMode.Multiple)
            {
                SpriteUtility.InitSpriteFactory(assetImporter);
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
