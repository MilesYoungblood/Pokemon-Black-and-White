using JetBrains.Annotations;
using UnityEngine;

namespace Scripts.Utility
{
    [UsedImplicitly]
    public static class MonoBehaviorUtility
    {
        [UsedImplicitly]
        public static void Activate(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour.gameObject.SetActive(true);
        }

        [UsedImplicitly]
        public static void Deactivate(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour.gameObject.SetActive(false);
        }
    }
}
