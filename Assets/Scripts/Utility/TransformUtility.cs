using UnityEngine;

namespace Scripts.Utility
{
    public static class TransformUtility
    {
        public static void DestroyChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
