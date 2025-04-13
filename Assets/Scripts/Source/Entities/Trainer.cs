using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    public class Trainer : Character, IBattler, IEnumerable<Pokemon>, ISavable
    {
        [SerializeField] private List<Pokemon> party;

        private TrainerController _trainerController;

        public static int MaxPartySize => 6;

        public List<Pokemon> Party
        {
            get => party;
            private set => party = value;
        }

        public Pokemon this[int index]
        {
            get => Party[index];
            set => Party[index] = value;
        }

        public static event Action OnNextPokemon;

        public static event Action OnDefeat;

        private void Awake()
        {
            _trainerController = GetComponent<TrainerController>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Party.ForEach(pokemon => pokemon.Init());
        }

        public void HandleFaint()
        {
            Party.RemoveAt(0);
        }

        public void HandleNextPokemon()
        {
            OnNextPokemon?.Invoke();
        }

        public void HandleDefeat()
        {
            OnDefeat?.Invoke();
            _trainerController.DestroyObjects();
        }

        public bool CanFight => Party.Count > 0;

        public Pokemon ActivePokemon => Party.First();

        public string Prefix => "The opposing ";

        public string[] StartingDialogue => new[]
        {
            $"{this} wants to battle!",
            $"{this} sent out {ActivePokemon}!"
        };

        public string BattleThemeKey => "Disc 1/28 - Battle! (Trainer)";

        public string VictoryThemeKey => "Disc 1/29 - Victory! (Trainer)";

        public IEnumerator<Pokemon> GetEnumerator()
        {
            return ((IEnumerable<Pokemon>)party).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"{CharacterClass} {Name}";
        }

        public object CaptureState()
        {
            return Party.Select(pokemon => pokemon.SaveData).ToArray();
        }

        public void RestoreState(object state)
        {
            Party = ((PokemonSaveData[])state).Select(data => new Pokemon(data)).ToList();
            if (Party.Count == 0)
            {
                _trainerController.DestroyObjects();
            }
        }
    }
}
