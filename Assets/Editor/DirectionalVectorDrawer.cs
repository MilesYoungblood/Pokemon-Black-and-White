/*
using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(DirectionalVectorAttribute))]
    public class DirectionalVectorDrawer : PropertyDrawer
    {
        private static readonly string[] DirectionNames = { "Right", "Left", "Up", "Down" };
        private static readonly Vector2[] Directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                // Find the current index based on the current Vector2 value
                var currentIndex = -1;
                var currentValue = property.vector2Value;
                for (var i = 0; i < Directions.Length; i++)
                {
                    if (currentValue != Directions[i])
                    {
                        continue;
                    }

                    currentIndex = i;
                    break;
                }

                // Draw the dropdown
                var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, DirectionNames);
                if (selectedIndex >= 0 && selectedIndex < Directions.Length)
                {
                    property.vector2Value = Directions[selectedIndex];
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use DirectionalVector with Vector2.");
            }
        }
    }
}
*/