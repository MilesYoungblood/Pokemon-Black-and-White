using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var disableIf = (DisableIfAttribute)attribute;
            var conditionField = property.serializedObject.FindProperty(disableIf.ConditionFieldName);

            if (conditionField is { propertyType: SerializedPropertyType.Boolean })
            {
                var previousGUIState = GUI.enabled;
                GUI.enabled = !conditionField.boolValue;

                EditorGUI.PropertyField(position, property, label, true);

                GUI.enabled = previousGUIState;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Error: Check your condition field.");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
