using UnityEngine;

namespace Scripts.Utility
{
    public class CustomLabelAttribute : PropertyAttribute
    {
        public CustomLabelAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }
}