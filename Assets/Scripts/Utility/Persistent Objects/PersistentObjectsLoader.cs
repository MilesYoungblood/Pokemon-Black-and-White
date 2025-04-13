using UnityEngine;

namespace Scripts.Utility
{
    public class PersistentObjectsLoader : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjects;

        private void Awake()
        {
            var existingObjects =
                FindObjectsByType<PersistentObjects>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (existingObjects.Length == 0) Instantiate(persistentObjects);
        }
    }
}