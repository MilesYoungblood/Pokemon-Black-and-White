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

        private void Awake()
        {
            enabled = false;
        }

        public ItemSlotUI Init(Item item)
        {
            name.text = item.ToString();
            quantity.text = $"x{item.Quantity}";
            return this;
        }

        public void SetSelected(bool selected, bool selectable)
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
