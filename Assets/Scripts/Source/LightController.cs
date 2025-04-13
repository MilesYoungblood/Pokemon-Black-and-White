using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Light2D))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private new Light2D light;

        private IEnumerator Fade(float endValue, float duration)
        {
            yield return DOTween.To(GetIntensity, SetIntensity, endValue, duration);
        }

        public IEnumerator FadeIn(float duration)
        {
            yield return Fade(0.0f, duration);
        }

        public IEnumerator FadeOut(float duration)
        {
            yield return Fade(1.0f, duration);
        }

        private float GetIntensity()
        {
            return light.intensity;
        }

        private void SetIntensity(float value)
        {
            light.intensity = value;
        }
    }
}
