using UnityEngine;

namespace Scripts.Utility
{
    public static class Compass
    {
        public static readonly Vector2Int[] CardinalDirections =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public static Vector2Int GetRandomDirection()
        {
            return CardinalDirections.RandomElement();
        }
    }
}