using System;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct VolatileStatusEffect
    {
        [SerializeField] private VolatileStatusCondition volatileStatusCondition;

        [SerializeField] private float prob;

        [SerializeField] private bool self;

        public VolatileStatusCondition VolatileStatusCondition => volatileStatusCondition;

        public float Prob => prob;

        public bool Self => self;
    }
}
