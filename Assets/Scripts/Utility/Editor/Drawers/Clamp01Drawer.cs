using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    /// <summary>
    /// Property drawer that enforces Clamp01 behavior.
    /// </summary>
    [CustomPropertyDrawer(typeof(Clamp01Attribute))]
    public class Clamp01Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                // Draw slider and clamp value between 0 and 1
                EditorGUI.BeginProperty(position, label, property);
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, 0.0f, 1.0f);
                property.floatValue = Mathf.Clamp01(property.floatValue);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Clamp01 with float properties only.");
            }
        }
    }
}
