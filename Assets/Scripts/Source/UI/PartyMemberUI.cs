using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class PartyMemberUI : MonoBehaviour, ISelectable
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI level;

        [SerializeField] private HPBar hpBar;

        public Pokemon Pokemon
        {
            set
            {
                name.text = value.ToString();
                level.text = $"Lvl. {value.Level.ToString()}";
                hpBar.HP = value.HP / (float)value.MaxHP;
            }
        }

        private void Awake()
        {
            enabled = false;
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
