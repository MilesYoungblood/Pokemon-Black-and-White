using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utility
{
    public class Node
    {
        public Vector3Int Position { get; }

        public float GScore { get; set; }

        public float FScore { get; set; }

        public float Cost { get; set; }

        [CanBeNull] public Node Parent { get; set; }

        public Node(Vector3Int position, float gScore, float fScore, float cost, [CanBeNull] Node parent = null)
        {
            Position = position;
            GScore = gScore;
            FScore = fScore;
            Cost = cost;
            Parent = parent;
        }
    }
}
