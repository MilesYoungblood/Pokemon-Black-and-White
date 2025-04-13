using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(_8DirectionalVectorAttribute))]
    public class _8DirectionalVector2IntDrawer : PropertyDrawer
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.one,
            Vector2Int.right,
            Vector2Int.down + Vector2Int.right,
            Vector2Int.down,
            -Vector2Int.one,
            Vector2Int.left,
            Vector2Int.up + Vector2Int.left
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                // Find the current index based on the current Vector2 value
                var currentIndex = -1;
                var currentValue = property.vector2IntValue;
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
                var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, EightWindCompassRose.FullNames);
                if (selectedIndex >= 0 && selectedIndex < Directions.Length)
                {
                    property.vector2IntValue = Directions[selectedIndex];
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use DirectionalVector with Vector2Int.");
            }
        }
    }
}
