using TMPro;
using UnityEngine;

namespace Scripts.Utility
{
    [DisallowMultipleComponent]
    public class SelectableItem : MonoBehaviour, ISelectable
    {
        [SerializeField] private TextMeshProUGUI label;

        public void Highlight(bool selected, bool selectable)
        {
            label.color = selectable ? selected ? Color.yellow : Color.white : Color.gray;
        }
    }
}
