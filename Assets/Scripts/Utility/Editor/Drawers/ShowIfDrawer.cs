using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    namespace Editor
    {
        [CustomPropertyDrawer(typeof(ShowIfAttribute))]
        public class ShowIfDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var showIf = (ShowIfAttribute)attribute;
                var conditionField = property.serializedObject.FindProperty(showIf.ConditionFieldName);

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

                // Hide the field if the condition is false
                if (!conditionField.boolValue)
                {
                    return;
                }

                // Draw the field normally
                EditorGUI.PropertyField(position, property, label, true);
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var showIf = (ShowIfAttribute)attribute;
                var conditionField = property.serializedObject.FindProperty(showIf.ConditionFieldName);

                // If the condition is false, return 0 height (hides the field)
                if (conditionField is { propertyType: SerializedPropertyType.Boolean, boolValue: false })
                {
                    return 0;
                }

                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }
    }
}
