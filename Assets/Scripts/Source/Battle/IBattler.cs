using System.Collections.Generic;

namespace Scripts.Source
{
    public interface IBattler
    {
        public void HandleFaint();

        public void HandleNextPokemon();

        public void HandleDefeat();

        public bool CanFight { get; }

        public Pokemon ActivePokemon { get; }

        public List<Pokemon> Party { get; }

        public string Prefix { get; }

        public string[] StartingDialogue { get; }

        public string BattleThemeKey { get; }

        public string VictoryThemeKey { get; }
    }
}
