using System.Collections;
using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class PokemonHUD : MonoBehaviour, ISelectable
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI level;

        [SerializeField] private HPBar hpBar;

        private Pokemon _pokemon;

        public Pokemon Pokemon
        {
            set
            {
                _pokemon = value;
                name.text = value.ToString();
                level.text = $"Lvl. {value.Level}";
                hpBar.HP = value.HP / value.MaxHP;
            }
        }

        public IEnumerator UpdateHP()
        {
            yield return hpBar.SetHPSmooth(_pokemon.HP / _pokemon.MaxHP);
        }

        public void Highlight(bool selected, bool selectable)
        {
            name.color = selected ? Color.yellow : Color.white;
        }
    }
}
