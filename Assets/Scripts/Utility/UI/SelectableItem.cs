using TMPro;
using UnityEngine;

namespace Scripts.Utility
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SelectableItem : MonoBehaviour, ISelectable
    {
        private TextMeshProUGUI[] _texts;

        private void Awake()
        {
            _texts = GetComponents<TextMeshProUGUI>();
        }

        public void SetSelected(bool selected, bool selectable)
        {
            foreach (var text in _texts)
            {
                text.color = selectable ? selected ? Color.yellow : Color.white : Color.gray;
            }
        }
    }
}
