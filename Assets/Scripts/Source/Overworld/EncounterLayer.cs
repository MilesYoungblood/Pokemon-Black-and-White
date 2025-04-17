using System;
using Scripts.Utility;
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
            return new Pokemon(wildPokemon.RandomElement()).Init();
        }

        public void OnTrigger()
        {
            if (new RangeInt(0, byte.MaxValue).RandomInt() < GetTileRange().RandomInt())
            {
                OnWildEncounter?.Invoke(GenerateWildPokemon());
            }
        }
    }
}
