using Scripts.Utility;
using TMPro;
using UnityEngine;

namespace Scripts.Source.UI
{
    public class ItemSlotUI : MonoBehaviour, ISelectable
    {
        [SerializeField] private new TextMeshProUGUI name;

        [SerializeField] private TextMeshProUGUI quantity;

        private RectTransform _rectTransform;

        public float Height => _rectTransform.rect.height;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
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

        public void SetSelected(bool selected)
        {
            throw new System.NotImplementedException();
        }
    }
}
