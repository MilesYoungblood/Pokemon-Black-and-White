using UnityEngine;

namespace Scripts.Utility
{
    public class PersistentObjects : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}