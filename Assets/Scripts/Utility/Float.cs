using System;
using UnityEngine;

namespace Scripts.Utility
{
    [Serializable]
    public struct Float : IEquatable<Float>, IEquatable<float>, IComparable<Float>
    {
        [SerializeField] private float value;

        public static bool operator ==(Float lhs, Float rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Float lhs, Float rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(Float lhs, float rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Float lhs, float rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(Float other)
        {
            return Mathf.Approximately(value, other.value);
        }

        public bool Equals(float other)
        {
            return Mathf.Approximately(value, other);
        }

        public int CompareTo(Float other)
        {
            return value.CompareTo(other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is Float other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}