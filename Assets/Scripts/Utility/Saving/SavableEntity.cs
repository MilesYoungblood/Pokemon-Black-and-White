using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scripts.Utility
{
    [ExecuteAlways]
    public class SavableEntity : MonoBehaviour
    {
        private static readonly Dictionary<string, SavableEntity> GlobalLookup = new();
        [SerializeField] private string uniqueId;

        private ISavable[] _savables;

        public string UniqueId => uniqueId;

        // Used to capture the state of the GameObject on which the savableEntity is attached
        private void Start()
        {
            _savables = GetComponents<ISavable>();
        }

#if UNITY_EDITOR
        // Update method used for generating UUID of the SavableEntity
        private void Update()
        {
            // don't execute in play-mode
            if (Application.IsPlaying(gameObject))
            {
                return;
            }

            // don't generate ID for prefabs (a prefab scene will have a path as null)
            if (string.IsNullOrEmpty(gameObject.scene.path))
            {
                return;
            }

            var serializedObject = new SerializedObject(this);
            var property = serializedObject.FindProperty("uniqueId");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            GlobalLookup[property.stringValue] = this;
        }
#endif

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();
            foreach (var savable in _savables)
            {
                state[savable.GetType().ToString()] = savable.CaptureState();
            }

            return state;
        }

        // Used to restore the state of the GameObject on which the savableEntity is attached
        public void RestoreState(object state)
        {
            var stateDict = (Dictionary<string, object>)state;
            foreach (var savable in GetComponents<ISavable>())
            {
                if (stateDict.TryGetValue(savable.GetType().ToString(), out var value))
                {
                    savable.RestoreState(value);
                }
            }
        }

        private bool IsUnique(string candidate)
        {
            if (!GlobalLookup.TryGetValue(candidate, out var value))
            {
                return true;
            }

            if (value == this)
            {
                return true;
            }

            // Handle scene unloading cases
            if (!GlobalLookup[candidate])
            {
                GlobalLookup.Remove(candidate);
                return true;
            }

            // Handle edge cases like designer manually changing the UUID
            if (GlobalLookup[candidate].UniqueId == candidate)
            {
                return false;
            }

            GlobalLookup.Remove(candidate);
            return true;
        }
    }
}
