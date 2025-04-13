using UnityEngine;

namespace Scripts.Utility
{
    [DisallowMultipleComponent]
    public class Cluster : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        [SerializeField] private Vector3 offset;

        [SerializeField] [Min(1)] private Vector3Int grid = Vector3Int.one;

        private void Awake()
        {
            enabled = false;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEditor.EditorApplication.delayCall += DelayedGeneration;
            }
#endif
        }

        private void DelayedGeneration()
        {
            if (this)
            {
                Generate();
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            if (!prefab)
            {
                return;
            }

            Clear();

            var range = (Vector3)grid / 2.0f + Vector3.Scale(
                Vector3.Max(grid - Vector3Int.one, Vector3.zero),
                offset
            );

            for (var z = 0; z < grid.z; ++z)
            {
                for (var y = 0; y < grid.y; ++y)
                {
                    for (var x = 0; x < grid.x; ++x)
                    {
                        Instantiate(
                            prefab,
                            transform.position + new Vector3(
                                Mathf.Lerp(-range.x, range.x, GetTime(x, grid.x)),
                                Mathf.Lerp(-range.y, range.y, GetTime(y, grid.y)),
                                Mathf.Lerp(-range.z, range.z, GetTime(z, grid.z))
                            ),
                            Quaternion.identity,
                            transform
                        ).name = prefab.name;
                    }
                }
            }
        }

        private static float GetTime(int index, int size)
        {
            return size > 1 ? index / (size - 1.0f) : 0.5f;
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            for (var i = transform.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}
