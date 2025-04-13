/*
using System;
using System.Reflection;
using Scripts;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class SerializedFieldEditor : UnityEditor.Editor
    {
        [MenuItem("Tools/Generic/Rename Serialized Field")]
        public static void RenameSerializedFieldInAssets()
        {
            // Call the generic method with specific types and field names
            RenameSerializedField<PokemonBase>("baseHp", "hp");
        }

        private static void RenameSerializedField<T>(string oldFieldName, string newFieldName) where T : ScriptableObject
        {
            // Get all assets of the specific type from the entire project
            var assetGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (var guid in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset == null)
                {
                    continue;
                }

                var serializedObject = new SerializedObject(asset);
                var oldProperty = serializedObject.FindProperty(oldFieldName);

                if (oldProperty != null)
                {
                    // Only proceed if the new property does not already exist
                    var newProperty = serializedObject.FindProperty(newFieldName);
                    if (newProperty != null)
                    {
                        continue;
                    }

                    // Rename the property in the serialized data
                    CopyProperty(serializedObject, oldProperty, newFieldName);

                    // Apply changes and save the asset
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning($"Field '{oldFieldName}' not found in asset at path: {path}");
                }
            }

            Debug.Log("Field renaming complete!");
        }

        private static void CopyProperty(SerializedObject serializedObject, SerializedProperty oldProperty,
            string newFieldName)
        {
            // Using reflection to set the value of the new field
            object targetObject = serializedObject.targetObject;
            var objectType = targetObject.GetType();
            var newFieldInfo = objectType.GetField(newFieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (newFieldInfo == null)
            {
                return;
            }

            switch (oldProperty.propertyType)
            {
                case SerializedPropertyType.String:
                    newFieldInfo.SetValue(targetObject, oldProperty.stringValue);
                    break;
                case SerializedPropertyType.Integer:
                    newFieldInfo.SetValue(targetObject, oldProperty.intValue);
                    break;
                case SerializedPropertyType.Float:
                    newFieldInfo.SetValue(targetObject, oldProperty.floatValue);
                    break;
                case SerializedPropertyType.Boolean:
                    newFieldInfo.SetValue(targetObject, oldProperty.boolValue);
                    break;
                case SerializedPropertyType.ObjectReference:
                    newFieldInfo.SetValue(targetObject, oldProperty.objectReferenceValue);
                    break;
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Color:
                    newFieldInfo.SetValue(targetObject, oldProperty.colorValue);
                    break;
                case SerializedPropertyType.LayerMask:
                    break;
                case SerializedPropertyType.Enum:
                    newFieldInfo.SetValue(targetObject, oldProperty.enumValueFlag);
                    break;
                case SerializedPropertyType.Vector2:
                    newFieldInfo.SetValue(targetObject, oldProperty.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    newFieldInfo.SetValue(targetObject, oldProperty.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    newFieldInfo.SetValue(targetObject, oldProperty.vector4Value);
                    break;
                case SerializedPropertyType.Rect:
                    newFieldInfo.SetValue(targetObject, oldProperty.rectValue);
                    break;
                case SerializedPropertyType.ArraySize:
                    newFieldInfo.SetValue(targetObject, oldProperty.arraySize);
                    break;
                case SerializedPropertyType.Character:
                    break;
                case SerializedPropertyType.AnimationCurve:
                    break;
                case SerializedPropertyType.Bounds:
                    break;
                case SerializedPropertyType.Gradient:
                    break;
                case SerializedPropertyType.Quaternion:
                    break;
                case SerializedPropertyType.ExposedReference:
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    break;
                case SerializedPropertyType.Vector2Int:
                    break;
                case SerializedPropertyType.Vector3Int:
                    break;
                case SerializedPropertyType.RectInt:
                    break;
                case SerializedPropertyType.BoundsInt:
                    break;
                case SerializedPropertyType.ManagedReference:
                    break;
                case SerializedPropertyType.Hash128:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Add other types as needed
        }
    }
}
*/