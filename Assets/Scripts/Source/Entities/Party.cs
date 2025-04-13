using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utility;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class Party : MonoBehaviour, IEnumerable<Pokemon>, ISavable
    {
        [SerializeField] private List<Pokemon> members;

        public static int MaxSize => 6;

        private List<Pokemon> Members
        {
            get => members;
            set => members = value;
        }

        public Pokemon this[int index]
        {
            get => Members[index];
            set => Members[index] = value;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Members.ForEach(pokemon => pokemon.Init());
        }

        public IEnumerator<Pokemon> GetEnumerator()
        {
            return ((IEnumerable<Pokemon>)Members)?.GetEnumerator() ?? throw new InvalidOperationException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object CaptureState()
        {
            return Members.Select(pokemon => pokemon.SaveData).ToArray();
        }

        public void RestoreState(object state)
        {
            Members = ((PokemonSaveData[])state).Select(data => new Pokemon(data)).ToList();
        }
    }
}
