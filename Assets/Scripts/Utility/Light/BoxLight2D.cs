using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scripts.Utility
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light2D))]
    public class BoxLight2D : MonoBehaviour
    {
        [SerializeField] private Vector2 size;

        [SerializeField] private Light2D light2D;

        public Vector2 Size
        {
            set => size = value;
        }

        private void Awake()
        {
            light2D = GetComponent<Light2D>();
            light2D.lightType = Light2D.LightType.Freeform;
        }

        private void OnValidate()
        {
            Awake();
            SetLighting();
        }

        public void SetLighting()
        {
            var x = size.x / 2.0f;
            var y = size.y / 2.0f;

            // Use reflection to set the shapePath field
            light2D.SetShapePath(new Vector3[]
            {
                new(-x, -y),
                new(x, -y),
                new(x, y),
                new(-x, y)
            });
        }
    }
}
