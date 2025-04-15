using System;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct StatEffect
    {
        private const int Sharply = 2;

        private const int Drastically = 3;

        private const int Immensely = 4;

        [SerializeField] private Stat stat;

        [SerializeField] [Range(-Immensely, Immensely)]
        private int amount;

        [SerializeField] [Clamp01] private float prob;

        [SerializeField] private bool self;

        public Stat Stat => stat;

        public int Amount => amount;

        public float Prob => prob;

        public bool Self => self;

        public string AmountText => Mathf.Abs(Amount) switch
        {
            Sharply => " sharply",
            Drastically => " drastically",
            Immensely => " immensely",
            _ => string.Empty
        };
    }
}
