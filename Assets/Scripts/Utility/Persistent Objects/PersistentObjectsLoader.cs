using UnityEngine;

namespace Scripts.Utility
{
    public class PersistentObjectsLoader : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjects;

        private void Awake()
        {
            if (FindObjectsByType<PersistentObjects>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                ).Length is 0)
            {
                Instantiate(persistentObjects);
            }
        }
    }
}
