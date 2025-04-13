using System;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct EffortValue
    {
        [SerializeField] private Stat stat;

        [SerializeField, Range(1, 3)] private int value;

        public Stat Stat => stat;

        public int Value => value;
    }
}
