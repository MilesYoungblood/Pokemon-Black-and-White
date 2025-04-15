using System;
using UnityEngine;

namespace Scripts.Source
{
    public sealed class StatModifier
    {
        private const int Limit = 6;

        private readonly bool _isBaseStat;

        private int _stage;

        public float Multiplier
        {
            get
            {
                if (_isBaseStat)
                {
                    return _stage < 0 ? 2.0f / (8 + _stage) : (2.0f + _stage) / 2.0f;
                }

                return _stage switch
                {
                    -6 => 33.0f,
                    -5 => 36.0f,
                    -4 => 43.0f,
                    -3 => 50.0f,
                    -2 => 60.0f,
                    -1 => 75.0f,
                    0 => 100.0f,
                    1 => 133.0f,
                    2 => 166.0f,
                    3 => 200.0f,
                    4 => 250.0f,
                    5 => 266.0f,
                    6 => 300.0f,
                    _ => throw new ArgumentOutOfRangeException()
                } / 100.0f;
            }
        }

        public StatModifier(bool isBaseStat)
        {
            _isBaseStat = isBaseStat;
        }

        public void Init()
        {
            _stage = 0;
        }

        public bool Adjust(int x)
        {
            var limit = Mathf.Abs(_stage) is Limit;
            _stage = Mathf.Clamp(_stage + x, -Limit, Limit);
            return !limit;
        }

        public static float operator *(float x, StatModifier statModifier)
        {
            return x * statModifier.Multiplier;
        }
    }
}
