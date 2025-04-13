using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class Pokedex : MonoBehaviour, ISavable
    {
        [SerializeField] private Sprite[] typeIcons;

        [SerializeField] private Sprite fakeSprite;

        private readonly Dictionary<string, PokemonAsset> _bases = new();

        private readonly HashSet<string> _registeredPokemon = new();

        public static Pokedex Instance { get; private set; }

        public Sprite FakeSprite => fakeSprite;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                foreach (var pokemonBase in Resources.LoadAll<PokemonAsset>("Pokemon"))
                {
                    if (!_bases.TryAdd(pokemonBase.name, pokemonBase))
                    {
                        print($"Unable to add Pokemon Base {pokemonBase.name}");
                    }
                }

                gameObject.SetActive(false);
            }
        }

        public PokemonAsset GetBaseByName(string pokemonName)
        {
            return _bases[pokemonName];
        }

        public Sprite GetTypeIcon(Type.ID type)
        {
            return typeIcons[(int)type - 1];
        }

        public bool RegisterPokemon(string pokemonName)
        {
            return _registeredPokemon.Add(pokemonName);
        }

        public bool IsRegistered(string pokemonName)
        {
            return _registeredPokemon.Contains(pokemonName);
        }

        public object CaptureState()
        {
            return _registeredPokemon.ToArray();
        }

        public void RestoreState(object state)
        {
            foreach (var pokemon in (string[])state)
            {
                if (!RegisterPokemon(pokemon))
                {
                    Debug.Log($"Unable to re-add \"{pokemon}\" to the Pokedex.");
                }
            }
        }
    }
}
