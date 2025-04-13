using System;
using UnityEngine;

namespace Scripts.Utility
{
    [Serializable]
    public abstract class Range<T> where T : struct, IComparable<T>, IEquatable<T>
    {
        [SerializeField] private T min;

        [SerializeField] private T max;

        public T Min
        {
            get => min;
            set => min = value;
        }

        public T Max
        {
            get => max;
            set => max = value;
        }

        protected Range(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException("Min value cannot be greater than Max value");
            }

            this.min = min;
            this.max = max;
        }

        public bool IsInRange(T value)
        {
            return 0 <= value.CompareTo(Min) && value.CompareTo(Max) <= 0;
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }
    }
}
