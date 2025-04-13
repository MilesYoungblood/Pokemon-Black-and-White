using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source.UI
{
    public class PartyMemberUI : MonoBehaviour, ISelectable
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI level;

        [SerializeField] private HPBar hpBar;

        public void Init(Pokemon pokemon)
        {
            name.text = pokemon.ToString();
            level.text = $"Lvl. {pokemon.Level}";
            hpBar.SetHp(pokemon.HP / (float)pokemon.MaxHP);
        }

        public void SetSelected(bool selected, bool selectable)
        {
            if (selected)
            {
                name.fontStyle |= FontStyles.Bold;
            }
            else
            {
                name.fontStyle &= ~FontStyles.Bold;
            }
        }
    }
}
