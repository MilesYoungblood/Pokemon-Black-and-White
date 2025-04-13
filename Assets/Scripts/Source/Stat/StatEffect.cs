using System;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct StatEffect
    {
        [SerializeField] private Stat stat;

        [SerializeField] private int amount;

        [SerializeField] [Clamp01] private float prob;

        [SerializeField] private bool self;

        public Stat Stat => stat;

        public int Amount => amount;

        public float Prob => prob;

        public bool Self => self;
    }
}
