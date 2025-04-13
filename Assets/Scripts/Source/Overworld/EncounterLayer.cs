using System;
using UnityEngine;
using RangeInt = Scripts.Utility.RangeInt;

namespace Scripts.Source
{
    public class EncounterLayer : MonoBehaviour, ITriggerable
    {
        public static event Action OnWildEncounter;

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

        public void OnTrigger(PlayerController playerController)
        {
            if (UnityEngine.Random.Range(0, byte.MaxValue + 1) < GetTileRange().RandomInt())
            {
                OnWildEncounter?.Invoke();
            }
        }
    }
}
