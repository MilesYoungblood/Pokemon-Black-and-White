using System;

namespace Scripts.Utility
{
    [Serializable]
    public class RangeFloat : Range<float>
    {
        public RangeFloat(float min, float max) : base(min, max)
        {
        }

        public float RandomFloat()
        {
            return UnityEngine.Random.Range(Min, Max + 1);
        }
    }
}
