using System;
using UnityEngine;

namespace Scripts.Utility
{
    [Serializable]
    public class SpriteSheet
    {
        [SerializeField] private Sprite[] data;

        [SerializeField] [Min(0)] private int rows;

        [SerializeField] [Min(0)] private int columns;

        [SerializeField] [Min(1)] private int frameRate;

        [SerializeField] [Min(0)] private float delay;

        [SerializeField] private bool loops;

        public Sprite this[int i, int j] =>
            i >= 0 && i < columns && j >= 0 && j < rows ? data[i + j * columns] : default;

        public int Rows => rows;

        public int Columns => columns;

        public int FrameRate => frameRate;

        public float Delay => delay;

        public bool Loops => loops;
    }
}