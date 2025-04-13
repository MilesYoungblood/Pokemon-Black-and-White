using System;
using System.Collections.Generic;

namespace Scripts.Source
{
    [Serializable]
    public struct PokemonSaveData
    {
        public string name;
        public string nickname;
        public int hp;
        public int level;
        public Nature.ID nature;
        public StatusCondition.ID statusCondition;
        public List<MoveSaveData> moves;
    }
}
