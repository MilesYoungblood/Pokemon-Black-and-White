using System;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct StatusEffect
    {
        [SerializeField] private StatusCondition.ID statusCondition;

        [SerializeField] private float prob;

        public StatusCondition.ID StatusCondition => statusCondition;

        public float Prob => prob;
    }
}
