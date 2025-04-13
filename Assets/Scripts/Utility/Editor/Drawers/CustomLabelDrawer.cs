using Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    [CustomPropertyDrawer(typeof(CustomLabelAttribute))]
    public class CustomLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var customLabel = (CustomLabelAttribute)attribute;

            // Replace the default label with the custom label
            label.text = customLabel.Label;

            // Draw the property field based on its type
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntField(position, label, property.intValue);
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                    break;

                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                    break;

                case SerializedPropertyType.Enum:
                    property.enumValueIndex = EditorGUI.Popup(
                        position,
                        label.text,
                        property.enumValueIndex,
                        property.enumDisplayNames
                    );
                    break;

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                default:
                    // For unsupported types, fall back to the default property field
                    EditorGUI.PropertyField(position, property, label);
                    break;
            }
        }
    }
}
