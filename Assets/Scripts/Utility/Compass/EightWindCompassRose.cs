using System;
using UnityEngine;

namespace Scripts.Utility
{
    public static class EightWindCompassRose
    {
        public const int NumDirections = 8;

        public static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.one,
            Vector2Int.right,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down,
            -Vector2Int.one,
            Vector2Int.left,
            Vector2Int.up + Vector2Int.left
        };

        private static readonly string[] Names =
        {
            "S",
            "SE",
            "E",
            "NE",
            "N",
            "NW",
            "W",
            "SW"
        };

        public static readonly string[] FullNames =
        {
            "North",
            "Northeast",
            "East",
            "Southeast",
            "South",
            "Southwest",
            "West",
            "Northwest"
        };

        public static string GetName(int i)
        {
            return Names[i];
        }

        public static Vector2Int GetDirection(int i)
        {
            return Directions[i];
        }

        public static (Vector2Int, Vector2Int) GetAdjacentDirections(Vector2Int direction)
        {
            var index = Array.IndexOf(Directions, direction);
            if (index == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return (
                Directions[(index - 1 + Directions.Length) % Directions.Length],
                Directions[(index + 1) % Directions.Length]
            );
        }
    }
}
