using System.Collections;
using TMPro;
using UnityEngine;

namespace Scripts.Source.UI
{
    public class PokemonHUD : MonoBehaviour
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI level;

        [SerializeField] private HPBar hpBar;

        private Pokemon _pokemon;

        public void Init(Pokemon pokemon)
        {
            _pokemon = pokemon;
            name.text = pokemon.ToString();
            level.text = $"Lvl. {pokemon.Level}";
            hpBar.SetHp(pokemon.HP / (float)pokemon.MaxHP);
        }

        public IEnumerator UpdateHp()
        {
            yield return hpBar.SetHpSmooth(_pokemon.HP / (float)_pokemon.MaxHP);
        }
    }
}
