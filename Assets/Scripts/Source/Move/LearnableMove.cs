using System;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct LearnableMove
    {
        [SerializeField] private MoveAsset asset;

        [SerializeField, Range(Pokemon.MinLevel, Pokemon.MaxLevel)]
        private int level;

        public MoveAsset Asset => asset;

        public int Level => level;
    }
}
