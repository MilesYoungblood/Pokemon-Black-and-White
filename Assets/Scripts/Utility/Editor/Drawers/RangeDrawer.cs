using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(Range<>), true)]
    public class RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property drawing
            EditorGUI.BeginProperty(position, label, property);

            // Draw the label for the entire Range field
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate the width for each field and its label
            const float labelWidth = 30.0f; // Width for the "Min" and "Max" labels
            var fieldWidth = (position.width - labelWidth * 2) / 2f;

            // Rectangles for the min label and field
            var minLabelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var minFieldRect = new Rect(position.x + labelWidth, position.y, fieldWidth, position.height);

            // Rectangles for the max label and field
            var maxLabelRect = new Rect(position.x + labelWidth + fieldWidth, position.y, labelWidth, position.height);
            var maxFieldRect = new Rect(position.x + labelWidth * 2 + fieldWidth, position.y, fieldWidth, position.height);

            // Draw the "Min" and "Max" labels
            EditorGUI.LabelField(minLabelRect, "Min");
            EditorGUI.LabelField(maxLabelRect, "Max");

            // Draw the min and max fields
            EditorGUI.PropertyField(minFieldRect, property.FindPropertyRelative("min"), GUIContent.none);
            EditorGUI.PropertyField(maxFieldRect, property.FindPropertyRelative("max"), GUIContent.none);

            // End property drawing
            EditorGUI.EndProperty();
        }
    }
}
