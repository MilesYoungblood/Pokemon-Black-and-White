using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var disableIf = (DisableIfAttribute)attribute;
            var conditionField = property.serializedObject.FindProperty(disableIf.ConditionFieldName);

            // Check if the condition field exists and is a boolean
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

            // Store the current GUI state
            var previousGUIState = GUI.enabled;

            // Disable the property field if conditionField is true
            GUI.enabled = !conditionField.boolValue;

            // Draw the property field
            EditorGUI.PropertyField(position, property, label, true);

            // Reset GUI enabled state back to previous
            GUI.enabled = previousGUIState;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
