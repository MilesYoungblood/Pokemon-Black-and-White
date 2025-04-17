using System;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct MovementPattern
    {
        [SerializeField] [_4DirectionalVector] private Vector2Int direction;

        [SerializeField] [Min(0)] private int displacement;

        [SerializeField] [Min(0.0f)] private float waitTime;

        public Vector2Int Direction => direction;

        public int Displacement => displacement;

        public float WaitTime => waitTime;
    }
}
