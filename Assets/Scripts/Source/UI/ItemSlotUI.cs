using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class ItemSlotUI : MonoBehaviour, ISelectable
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI quantity;

        [SerializeField] private RectTransform rectTransform;

        public float Height => rectTransform.rect.height;

        public Item Item
        {
            set
            {
                name.text = value.ToString();
                quantity.text = value.Quantity.ToString();
            }
        }

        private void Awake()
        {
            enabled = false;
        }

        public void Highlight(bool selected, bool selectable)
        {
            if (selected)
            {
                name.fontStyle |= FontStyles.Bold;
                quantity.fontStyle |= FontStyles.Bold;
            }
            else
            {
                name.fontStyle &= ~FontStyles.Bold;
                quantity.fontStyle |= FontStyles.Bold;
            }
        }
    }
}
