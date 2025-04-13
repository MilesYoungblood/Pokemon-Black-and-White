using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public sealed class Pokemon : IBattler
    {
        #region Constants

        public const int MaxMoveSetSize = 4;

        private const int MinHP = 0;

        public const int MinLevel = 1;

        public const int MaxLevel = 100;

        #endregion

        #region Serialized Fields

        [SerializeField] private PokemonAsset asset;

        [SerializeField, Range(MinLevel, MaxLevel)]
        private int level;

        [SerializeField] private List<Move> moveSet;

        #endregion

        #region Fields

        private int _hp;

        #endregion

        #region Properties

        public PokemonAsset Asset
        {
            get => asset;
            private set => asset = value;
        }

        public string Nickname { get; set; }

        public int Level
        {
            get => level;
            private set => level = Mathf.Clamp(value, Asset.MinLevel, MaxLevel);
        }

        public Nature.ID Nature { get; private set; }

        [CanBeNull] public StatusCondition StatusCondition { get; set; }

        public List<Move> MoveSet
        {
            get => moveSet;
            set => moveSet = value;
        }

        public int HP
        {
            get => _hp;
            set => _hp = Mathf.Clamp(value, MinHP, MaxHP);
        }

        public int MaxHP { get; private set; }

        public int Attack { get; private set; }

        public int Defense { get; private set; }

        public int SpAttack { get; private set; }

        public int SpDefense { get; private set; }

        public int Speed { get; private set; }

        public PokemonSaveData SaveData => new()
        {
            name = Asset.name,
            nickname = Nickname,
            hp = HP,
            level = Level,
            nature = Nature,
            statusCondition = StatusCondition?.GetID() ?? StatusCondition.ID.None,
            moves = MoveSet.Select(move => move.SaveData).ToList()
        };

        #endregion

        #region Indexers

        public Move this[int index]
        {
            get => MoveSet[index];
            set => MoveSet[index] = value;
        }

        #endregion

        #region Events

        public event Action OnBurned;

        public event Action OnParalyzed;

        public event Action OnFrozen;

        #endregion

        #region Methods

        public Pokemon(PokemonSaveData saveData)
        {
            Asset = Pokedex.Instance.GetBaseByName(saveData.name);
            Nickname = saveData.nickname;
            Level = saveData.level;
            // TODO watch out for logic error here
            CalculateBaseStats();
            HP = saveData.hp;
            Nature = saveData.nature;
            StatusCondition = StatusCondition.GetConditionByID(saveData.statusCondition);
            MoveSet = saveData.moves.Select(data => new Move(data)).ToList();
        }

        public Pokemon(Pokemon pokemon) : this(pokemon.SaveData)
        {
        }

        public void Init()
        {
            if (string.IsNullOrEmpty(Nickname))
            {
                Nickname = Asset.name;
            }

            // nature must be determined before stats are calculated since base stats partially rely on nature
            Nature = (Nature.ID)UnityEngine.Random.Range(0, 25);

            CalculateBaseStats();
            HP = MaxHP;
            MoveSet = MoveSet.Take(MaxMoveSetSize).ToList();
            MoveSet.ForEach(move => move.Init());
        }

        private void CalculateBaseStats()
        {
            // find initial calculations
            var initialStats = new Dictionary<Stat, float>
            {
                [Stat.Hp] = StatCalc(Asset.Hp, true),
                [Stat.Attack] = StatCalc(Asset.Attack, false),
                [Stat.Defense] = StatCalc(Asset.Defense, false),
                [Stat.SpAttack] = StatCalc(Asset.SpAttack, false),
                [Stat.SpDefense] = StatCalc(Asset.SpDefense, false),
                [Stat.Speed] = StatCalc(Asset.Speed, false)
            };

            // find the boosted and lowered natures
            var boosted = Source.Nature.BoostedStat(Nature);
            var lowered = Source.Nature.LoweredStat(Nature);

            // if the nature isn't a neutral nature, apply nature effects
            if (boosted != lowered)
            {
                initialStats[boosted] *= 1.1f;
                initialStats[lowered] *= 0.9f;
            }

            // apply final calculations
            MaxHP = Mathf.FloorToInt(initialStats[Stat.Hp]);
            Attack = Mathf.FloorToInt(initialStats[Stat.Attack]);
            Defense = Mathf.FloorToInt(initialStats[Stat.Defense]);
            SpAttack = Mathf.FloorToInt(initialStats[Stat.SpAttack]);
            SpDefense = Mathf.FloorToInt(initialStats[Stat.SpDefense]);
            Speed = Mathf.FloorToInt(initialStats[Stat.Speed]);
        }

        private float StatCalc(int stat, bool hp)
        {
            return 2.0f * stat * Level / 100.0f + (hp ? 10 : 5);
        }

        public float SpeedCalc()
        {
            return Speed * (StatusCondition is Paralysis ? 0.5f : 1.0f);
        }

        public bool IsFasterThan(Pokemon rival)
        {
            return SpeedCalc() > rival.SpeedCalc();
        }

        public bool RivalsInSpeed(Pokemon rival)
        {
            return Mathf.Approximately(SpeedCalc(), rival.SpeedCalc());
        }

        public void LevelUp()
        {
            ++Level;
            CalculateBaseStats();
        }

        public void LearnMove(Move move)
        {
            if (MoveSet.Count < MaxMoveSetSize)
            {
                MoveSet.Add(move);
            }
        }

        public void ForgetMove(int index)
        {
            if (MoveSet.Count > 1)
            {
                MoveSet.RemoveAt(index);
            }
        }

        public Move ChooseRandomMove()
        {
            // TODO replace with struggle
            if (MoveSet.All(move => !move.CanUse()))
            {
                return new Move();
            }

            Move move;
            do
            {
                move = MoveSet[UnityEngine.Random.Range(0, MoveSet.Count)];
            } while (!move.CanUse());

            return move;
        }

        public bool HasType(Type.ID type)
        {
            return Asset.Type1 == type || Asset.Type2 == type;
        }

        public void InflictBurn()
        {
            StatusCondition = new Burn();
            OnBurned?.Invoke();
        }

        public void InflictParalysis()
        {
            StatusCondition = new Paralysis();
            OnParalyzed?.Invoke();
        }

        public void InflictFreeze()
        {
            StatusCondition = new Freeze();
            OnFrozen?.Invoke();
        }

        public void PlayCry()
        {
            AudioManager.Instance.PlaySound(Asset.GetDexAsString(false));
        }

        public IEnumerator PlayCryToCompletion()
        {
            yield return null;
            yield return AudioManager.Instance.PlaySoundToCompletion(Asset.GetDexAsString(false));
        }

        #endregion

        #region Interface Implementations

        public void HandleFaint()
        {
        }

        public void HandleNextPokemon()
        {
        }

        public void HandleDefeat()
        {
        }

        public bool CanFight => HP > 0;

        public Pokemon ActivePokemon => this;

        public List<Pokemon> Party => new() { this };

        public string Prefix => "The wild ";

        public string[] StartingDialogue => new[] { $"A wild {this} appeared!" };

        public string BattleThemeKey => "Disc 1/15 - Battle! (Wild Pokémon)";

        public string VictoryThemeKey => "Disc 1/16 - Victory! (Wild Pokémon)";

        #endregion

        public override string ToString()
        {
            return Nickname;
        }
    }
}
