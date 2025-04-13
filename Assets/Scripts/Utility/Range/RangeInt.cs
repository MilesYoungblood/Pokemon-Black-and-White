using System;

namespace Scripts.Utility
{
    [Serializable]
    public class RangeInt : Range<int>
    {
        public RangeInt(int min, int max) : base(min, max)
        {
        }

        public int RandomInt()
        {
            return UnityEngine.Random.Range(Min, Max + 1);
        }
    }
}
