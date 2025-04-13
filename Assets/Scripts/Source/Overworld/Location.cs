using UnityEngine;

namespace Scripts.Source
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private Pokemon[] wildPokemon;

        public Pokemon GenerateWildPokemon()
        {
            var pokemon = new Pokemon(wildPokemon[Random.Range(0, wildPokemon.Length)]);
            pokemon.Init();
            return pokemon;
        }
    }
}
