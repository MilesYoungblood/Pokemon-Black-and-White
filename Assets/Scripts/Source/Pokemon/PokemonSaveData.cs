using System;

namespace Scripts.Source
{
    [Serializable]
    public struct PokemonSaveData
    {
        public string name;
        public string nickname;
        public float hp;
        public int level;
        public Nature.ID nature;
        public StatusCondition.ID statusCondition;
        public MoveSaveData[] moves;
    }
}
