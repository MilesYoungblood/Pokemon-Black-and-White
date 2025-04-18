﻿using System.Collections.Generic;
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
            }
        }

        public Sprite GetTypeIcon(Type.ID type)
        {
            return typeIcons[(int)type];
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
                    Debug.LogWarning($"Unable to re-add \"{pokemon}\" to the Pokedex.");
                }
            }
        }
    }
}
