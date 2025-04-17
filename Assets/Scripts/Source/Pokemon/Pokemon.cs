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

        #region Fields

        #region Serialized Fields

        [SerializeField] private PokemonAsset asset;

        [SerializeField] private string nickname;

        [SerializeField] [Range(MinLevel, MaxLevel)]
        private int level = MinLevel;

        [SerializeField] private List<Move> moveset;

        #endregion

        private float _hp;

        #endregion

        #region Properties

        public PokemonAsset Asset
        {
            get => asset;
            private set => asset = value;
        }

        public string Nickname
        {
            get => nickname;
            set => nickname = value;
        }

        public int Level
        {
            get => level;
            set
            {
                level = Mathf.Clamp(value, Asset.MinLevel, MaxLevel);
                CalculateBaseStats();
            }
        }

        public Nature.ID Nature { get; set; }

        [CanBeNull] public StatusCondition StatusCondition { get; set; }

        public List<Move> Moveset
        {
            get => moveset;
            set => moveset = value;
        }

        public float HP
        {
            get => _hp;
            set => _hp = Mathf.Clamp(value, MinHP, MaxHP);
        }

        public float MaxHP { get; private set; }

        public float Attack { get; private set; }

        public float Defense { get; private set; }

        public float SpAttack { get; private set; }

        public float SpDefense { get; private set; }

        public float Speed { get; private set; }

        public PokemonSaveData SaveData => new()
        {
            name = Asset.name,
            nickname = Nickname,
            hp = HP,
            level = Level,
            nature = Nature,
            statusCondition = StatusCondition?.GetID() ?? StatusCondition.ID.None,
            moves = Moveset.Select(move => move.SaveData).ToArray()
        };

        #endregion

        #region Indexers

        public Move this[int index]
        {
            get => Moveset[index];
            set => Moveset[index] = value;
        }

        public float this[Stat stat]
        {
            get
            {
                return stat switch
                {
                    Stat.HP => MaxHP,
                    Stat.Attack => Attack,
                    Stat.Defense => Defense,
                    Stat.SpAttack => SpAttack,
                    Stat.SpDefense => SpDefense,
                    Stat.Speed => Speed,
                    Stat.Accuracy => 1.0f,
                    Stat.Evasiveness => 1.0f,
                    _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
                };
            }
            set
            {
                switch (stat)
                {
                    case Stat.HP:
                        MaxHP = value;
                        break;
                    case Stat.Attack:
                        Attack = value;
                        break;
                    case Stat.Defense:
                        Defense = value;
                        break;
                    case Stat.SpAttack:
                        SpAttack = value;
                        break;
                    case Stat.SpDefense:
                        SpDefense = value;
                        break;
                    case Stat.Speed:
                        Speed = value;
                        break;
                    case Stat.Accuracy:
                        break;
                    case Stat.Evasiveness:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
                }
            }
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
            Asset = PokemonAsset.GetBaseByName(saveData.name);
            Nickname = saveData.nickname;
            Level = saveData.level;
            HP = saveData.hp;
            Nature = saveData.nature;
            StatusCondition = StatusCondition.GetConditionByID(saveData.statusCondition);
            Moveset = saveData.moves.Select(data => new Move(data)).ToList();
        }

        public Pokemon(Pokemon pokemon) : this(pokemon.SaveData)
        {
        }

        public Pokemon Init()
        {
            //if (string.IsNullOrEmpty(Nickname))
            {
                Nickname = Asset.name;
            }

            // nature must be determined before stats are calculated since base stats partially rely on nature
            //Nature = Source.Nature.GetRandomNature();

            Level = Mathf.Max(MinLevel, Level);
            HP = MaxHP;
            Moveset = Moveset.Take(MaxMoveSetSize).ToList();
            Moveset.ForEach(move => move.Init());
            return this;
        }

        private void CalculateBaseStats()
        {
            // apply final calculations
            MaxHP = StatCalc(Asset.HP, true);
            Attack = StatCalc(Asset.Attack, false);
            Defense = StatCalc(Asset.Defense, false);
            SpAttack = StatCalc(Asset.SpAttack, false);
            SpDefense = StatCalc(Asset.SpDefense, false);
            Speed = StatCalc(Asset.Speed, false);

            // apply nature effects
            this[Source.Nature.BoostedStat(Nature)] *= 1.1f;
            this[Source.Nature.LoweredStat(Nature)] *= 0.9f;

            this[Stat.HP] = Mathf.Round(this[Stat.HP]);
            this[Stat.Attack] = Mathf.Round(this[Stat.Attack]);
            this[Stat.Defense] = Mathf.Round(this[Stat.Defense]);
            this[Stat.SpAttack] = Mathf.Round(this[Stat.SpAttack]);
            this[Stat.SpDefense] = Mathf.Round(this[Stat.SpDefense]);
            this[Stat.Speed] = Mathf.Round(this[Stat.Speed]);
        }

        private float StatCalc(int stat, bool hp)
        {
            return 2.0f * stat * Level / MaxLevel + (hp ? 10 : 5);
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
        }

        public void LearnMove(Move move)
        {
            if (Moveset.Count < MaxMoveSetSize)
            {
                Moveset.Add(move);
            }
        }

        public void ForgetMove(int index)
        {
            if (Moveset.Count > 1)
            {
                Moveset.RemoveAt(index);
            }
        }

        public Move ChooseRandomMove()
        {
            // TODO replace with struggle
            return Moveset.All(move => !move.CanUse()) ? new Move() : Moveset.Where(move => move.CanUse()).RandomElement();
        }

        public bool HasType(Type.ID type)
        {
            return Asset.Type1 == type || Asset.Type2 == type;
        }

        public bool IsImmuneToStatus(StatusCondition.ID statusCondition)
        {
            return statusCondition switch
            {
                StatusCondition.ID.None => false,
                StatusCondition.ID.Burn => HasType(Type.ID.Fire),
                StatusCondition.ID.Paralysis => HasType(Type.ID.Electric),
                StatusCondition.ID.Freeze => HasType(Type.ID.Ice),
                StatusCondition.ID.Poison => HasType(Type.ID.Poison),
                StatusCondition.ID.BadPoison => HasType(Type.ID.Poison),
                StatusCondition.ID.Sleep => false,
                _ => throw new ArgumentOutOfRangeException(nameof(statusCondition), statusCondition, null)
            };
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

        public bool CanFight => !Mathf.Approximately(HP, 0);

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
