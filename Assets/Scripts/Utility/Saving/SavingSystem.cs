using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Scripts.Utility
{
    public static class SavingSystem
    {
        private static Dictionary<string, object> _gameState = new();

        public static void CaptureEntityStates(IEnumerable<SavableEntity> savableEntities)
        {
            foreach (var savable in savableEntities)
            {
                _gameState[savable.UniqueId] = savable.CaptureState();
            }
        }

        public static void RestoreEntityStates(IEnumerable<SavableEntity> savableEntities)
        {
            foreach (var savable in savableEntities)
            {
                if (_gameState.TryGetValue(savable.UniqueId, out var value))
                {
                    savable.RestoreState(value);
                }
            }
        }

        public static void Save(string saveFile)
        {
            CaptureState(_gameState);
            SaveFile(saveFile, _gameState);
        }

        public static void Load(string saveFile)
        {
            _gameState = LoadFile(saveFile);
            RestoreState(_gameState);
        }

        public static void Delete(string saveFile)
        {
            File.Delete(GetPath(saveFile));
        }

        // Used to capture states of all savable objects in the game
        private static void CaptureState(IDictionary<string, object> state)
        {
            foreach (var savable in Object.FindObjectsByType<SavableEntity>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                state[savable.UniqueId] = savable.CaptureState();
            }
        }

        // Used to restore states of all savable objects in the game
        private static void RestoreState(IReadOnlyDictionary<string, object> state)
        {
            foreach (var savable in Object.FindObjectsByType<SavableEntity>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                if (state.TryGetValue(savable.UniqueId, out var value))
                {
                    savable.RestoreState(value);
                }
            }
        }

        private static void SaveFile(string saveFile, Dictionary<string, object> state)
        {
            var path = GetPath(saveFile);
            MonoBehaviour.print($"saving to {path}");

            using var fs = File.Open(path, FileMode.Create);
            // Serialize our object
            new BinaryFormatter().Serialize(fs, state);
        }

        private static Dictionary<string, object> LoadFile(string saveFile)
        {
            var path = GetPath(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using var fs = File.Open(path, FileMode.Open);
            // Deserialize our object
            return (Dictionary<string, object>)new BinaryFormatter().Deserialize(fs);
        }

        private static string GetPath(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile);
        }
    }
}
