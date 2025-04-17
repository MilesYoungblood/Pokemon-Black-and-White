using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class Trainer : Entity, IList<Pokemon>, IBattler, ISavable
    {
        public const int MaxPartySize = 6;

        [SerializeField] private List<Pokemon> party;

        public static event Action OnNextPokemon;

        public static event Action OnDefeat;

        private void OnValidate()
        {
            foreach (var pokemon in Party)
            {
                if (pokemon.Asset)
                {
                    pokemon.Init();
                }
                else
                {
                    pokemon.Nickname = "";
                    pokemon.Nature = Nature.GetRandomNature();
                    pokemon.Level = Pokemon.MinLevel;
                    pokemon.Moveset.Clear();
                }
            }
        }

        protected virtual void Start()
        {
            foreach (var pokemon in party)
            {
                pokemon.Init();
            }
        }

        public int IndexOf(Pokemon item)
        {
            return party.IndexOf(item);
        }

        public void Insert(int index, Pokemon item)
        {
            party.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            party.RemoveAt(index);
        }

        public Pokemon this[int index]
        {
            get => party[index];
            set => party[index] = value;
        }

        public IEnumerator<Pokemon> GetEnumerator()
        {
            return party.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Pokemon item)
        {
            if (party.Count < MaxPartySize)
            {
                party.Add(item);
            }
        }

        public void Clear()
        {
            party.Clear();
        }

        public bool Contains(Pokemon item)
        {
            return party.Contains(item);
        }

        public void CopyTo(Pokemon[] array, int arrayIndex)
        {
            party.CopyTo(array, arrayIndex);
        }

        public bool Remove(Pokemon item)
        {
            return party.Remove(item);
        }

        public int Count => party.Count;

        public bool IsReadOnly => false;

        public void HandleFaint()
        {
            RemoveAt(0);
        }

        public void HandleNextPokemon()
        {
            OnNextPokemon?.Invoke();
        }

        public void HandleDefeat()
        {
            OnDefeat?.Invoke();
            transform.DestroyChildren();
        }

        public bool CanFight => this.Any();

        public Pokemon ActivePokemon => this.First();

        public List<Pokemon> Party => party;

        public string Prefix => "The opposing ";

        public string[] StartingDialogue => new[]
        {
            $"{this} wants to battle!",
            $"{this} sent out {ActivePokemon}!"
        };

        public string BattleThemeKey => "Disc 1/28 - Battle! (Trainer)";

        public string VictoryThemeKey => "Disc 1/29 - Victory! (Trainer)";

        public override string ToString()
        {
            return $"{CharacterClass} {Name}";
        }

        public object CaptureState()
        {
            return this.Select(pokemon => pokemon.SaveData).ToArray();
        }

        public void RestoreState(object state)
        {
            party = ((PokemonSaveData[])state).Select(data => new Pokemon(data)).ToList();
            if (!this.Any())
            {
                transform.DestroyChildren();
            }
        }
    }
}
