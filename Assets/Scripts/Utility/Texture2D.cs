using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Scripts.Utility
{
    public static class Texture2D
    {
        public static Vector2Int? FindFirstPixel([NotNull] UnityEngine.Texture2D texture, Color32 target)
        {
            var pixels = texture.GetPixels32(); // Faster than GetPixels()

            var width = texture.width;
            var height = texture.height;

            // Iterate through pixels (bottom to top, left to right)
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    if (pixels[y * width + x].Equals(target)) // Compare colors
                    {
                        return new Vector2Int(x, y); // Return pixel position
                    }
                }
            }

            return null; // No matching pixel found
        }

        public static Vector3 PixelToWorld(Sprite sprite, Vector2Int pixelPosition)
        {
            var spriteSize = new Vector2(sprite.texture.width, sprite.texture.height);
            var pivotOffset = sprite.pivot / spriteSize; // Get pivot offset (0 to 1 range)

            var normalizedPos = new Vector2(
                pixelPosition.x / spriteSize.x - pivotOffset.x,
                pixelPosition.y / spriteSize.y - pivotOffset.y
            );

            return new Vector3(normalizedPos.x * sprite.bounds.size.x, normalizedPos.y * sprite.bounds.size.y);
        }

        public static int FindLowestNonTransparentPixel([NotNull] UnityEngine.Texture2D texture)
        {
            if (!texture.isReadable)
            {
                Debug.LogError($"{texture} is not readable.");
                return -1;
            }

            var width = texture.width;
            var height = texture.height;
            var pixels = texture.GetPixels();

            // Scan from bottom to top
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    if (pixels[x + y * width].a > 0)
                    {
                        return y;
                    }
                }
            }

            Debug.Log($"{texture} has no transparent pixels.");
            return -1;
        }

        public static Sprite[] CreateSpriteSheet(
            this UnityEngine.Texture2D texture,
            int rows,
            int columns,
            int width,
            int height,
            float ppu)
        {
            var spriteSheet = new Sprite[rows * columns]; // Single array

            var pivot = Vector2.one / 2.0f;

            for (var i = 0; i < rows; ++i)
            {
                for (var j = 0; j < columns; ++j)
                {
                    var x = j * width;
                    var y = (rows - i - 1) * height;

                    var index = i * columns + j; // Flatten 2D indices to 1D

                    spriteSheet[index] = Sprite.Create(
                        texture,
                        new Rect(x, y, width, height),
                        pivot,
                        ppu
                    );
                    spriteSheet[index].name = $"{texture.name} [{i}, {j}]";
                }
            }

            return spriteSheet;
        }
    }
}
