using System;
using UnityEngine;
using RangeInt = Scripts.Utility.RangeInt;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class EncounterLayer : MonoBehaviour, ITriggerable
    {
        [SerializeField] private Pokemon[] wildPokemon;

        public static event Action<IBattler> OnWildEncounter;

        private RangeInt GetTileRange()
        {
            if (CompareTag("Tall grass"))
            {
                return new RangeInt(15, 25);
            }

            if (CompareTag("Cave"))
            {
                return new RangeInt(10, 15);
            }

            if (CompareTag("Water"))
            {
                return new RangeInt(5, 5);
            }

            throw new Exception("Invalid tag value.");
        }

        private Pokemon GenerateWildPokemon()
        {
            var pokemon = new Pokemon(wildPokemon[UnityEngine.Random.Range(0, wildPokemon.Length)]);
            pokemon.Init();
            return pokemon;
        }

        public void OnTrigger(PlayerController playerController)
        {
            if (UnityEngine.Random.Range(0, byte.MaxValue + 1) < GetTileRange().RandomInt())
            {
                OnWildEncounter?.Invoke(GenerateWildPokemon());
            }
        }
    }
}
