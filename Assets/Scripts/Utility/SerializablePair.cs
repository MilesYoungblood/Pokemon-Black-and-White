using System;

namespace Scripts.Utility
{
    [Serializable]
    public record SerializablePair<T1, T2>
    {
        public T1 key;

        public T2 value;
    }
}
