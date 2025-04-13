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

        [SerializeField, Min(0)] private float height;

        [SerializeField, Min(0)] private float weight;

        [SerializeField] private Type.ID type1;

        [SerializeField] private Type.ID type2;

        [SerializeField] private int dex;

        [Header("Base Stats")]
        [SerializeField, Min(0)]
        private int hp;

        [SerializeField, Min(0)] private int attack;

        [SerializeField, Min(0)] private int defense;

        [SerializeField, Min(0)] private int spAttack;

        [SerializeField, Min(0)] private int spDefense;

        [SerializeField, Min(0)] private int speed;

        [Header("Training")]
        [SerializeField, Range(1, Pokemon.MaxLevel)]
        private int minLevel;

        [SerializeField] private EffortValue evYield;

        [SerializeField, Range(0, byte.MaxValue)]
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

        public string Species => species;

        public float Height => height;

        public float Weight => weight;

        public Type.ID Type1 => type1;

        public Type.ID Type2 => type2;

        public int Hp => hp;

        public int Attack => attack;

        public int Defense => defense;

        public int SpAttack => spAttack;

        public int SpDefense => spDefense;

        public int Speed => speed;

        public int MinLevel => minLevel;

        public EffortValue EvYield => evYield;

        public int CatchRate => catchRate;

        public LearnableMove[] LearnableMoves => learnableMoves;

        public MoveAsset[] MachineMoves => machineMoves;

        public Sprite[] FrontSpriteSheet => frontSpriteSheet;

        public Sprite[] BackSpriteSheet => backSpriteSheet;

        public bool Levitates => levitates;

        public string GetDexAsString(bool localDex)
        {
            return (dex + (localDex ? 0 : UnovaStartingDex)).ToString("D3");
        }
    }
}
