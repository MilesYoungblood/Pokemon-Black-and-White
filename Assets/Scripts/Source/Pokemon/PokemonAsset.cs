using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Source
{
    [CreateAssetMenu(menuName = "Pokemon/Create new Pokemon")]
    public class PokemonAsset : ScriptableObject
    {
        private const int UnovaStartingDex = 494;

        [Header("Pokédex Data")]
        [SerializeField]
        private string species;

        [SerializeField] [Min(0)] private float height;

        [SerializeField] [Min(0)] private float weight;

        [SerializeField] private Type.ID type1;

        [SerializeField] private Type.ID type2;

        [SerializeField] private int dex;

        [Header("Base Stats")]
        [SerializeField]
        [Min(0)]
        private int hp;

        [SerializeField] [Min(0)] private int attack;

        [SerializeField] [Min(0)] private int defense;

        [SerializeField] [Min(0)] private int spAttack;

        [SerializeField] [Min(0)] private int spDefense;

        [SerializeField] [Min(0)] private int speed;

        [Header("Training")]
        [SerializeField]
        [Range(Pokemon.MinLevel, Pokemon.MaxLevel)]
        private int minLevel;

        [SerializeField] private EffortValue evYield;

        [SerializeField]
        [Range(0, byte.MaxValue)]
        private int catchRate;

        [Header("Moves")]
        [SerializeField]
        private LearnableMove[] learnableMoves;

        [SerializeField] private MoveAsset[] machineMoves;

        [Header("Sprites")]
        [SerializeField]
        private Sprite[] frontSpriteSheet;

        [SerializeField] private Sprite[] backSpriteSheet;

        [SerializeField] private bool levitates;

        private static readonly Dictionary<string, PokemonAsset> Bases = new();

        public string Species => species;

        public float Height => height;

        public float Weight => weight;

        public Type.ID Type1 => type1;

        public Type.ID Type2 => type2;

        public int HP => hp;

        public int Attack => attack;

        public int Defense => defense;

        public int SpAttack => spAttack;

        public int SpDefense => spDefense;

        public int Speed => speed;

        public int MinLevel => minLevel;

        public EffortValue EvYield => evYield;

        public int CatchRate => catchRate;

        public IReadOnlyList<LearnableMove> LearnableMoves => learnableMoves;

        public IReadOnlyList<MoveAsset> MachineMoves => machineMoves;

        public IReadOnlyList<Sprite> FrontSpriteSheet => frontSpriteSheet;

        public IReadOnlyList<Sprite> BackSpriteSheet => backSpriteSheet;

        public bool Levitates => levitates;

        private void OnEnable()
        {
            Bases.Add(name, this);
        }

        private void OnDisable()
        {
            Bases.Remove(name);
        }

        public string GetDexAsString(bool localDex)
        {
            return (dex + (localDex ? 0 : UnovaStartingDex)).ToString("D3");
        }

        public static PokemonAsset GetBaseByName(string pokemonName)
        {
            return Bases[pokemonName];
        }
    }
}
