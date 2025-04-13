using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hideIf = (HideIfAttribute)attribute;
            var conditionField = property.serializedObject.FindProperty(hideIf.ConditionFieldName);

            // If the condition field is missing, show an error
            if (conditionField == null)
            {
                EditorGUI.LabelField(position, label.text, "Error: Condition field not found.");
                return;
            }

            if (conditionField.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Error: Condition field must be a boolean.");
                return;
            }

            // Hide the field if the condition is true
            if (conditionField.boolValue)
            {
                return;
            }

            // Draw the field normally
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hideIf = (HideIfAttribute)attribute;
            var conditionField = property.serializedObject.FindProperty(hideIf.ConditionFieldName);

            // If the condition is met, return 0 height (hides the field)
            if (conditionField is { propertyType: SerializedPropertyType.Boolean, boolValue: true })
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
