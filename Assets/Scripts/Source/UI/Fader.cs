using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class Fader : MonoBehaviour
    {
        [SerializeField] private Image image;

        public Color RGB
        {
            set => image.color = new Color(value.r, value.g, value.b, image.color.a);
        }

        public float Alpha
        {
            set => image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(value));
        }

        public IEnumerator FadeIn(float time)
        {
            yield return image.DOFade(1.0f, time).WaitForCompletion();
        }

        public IEnumerator FadeOut(float time)
        {
            yield return image.DOFade(0.0f, time).WaitForCompletion();
        }
    }
}
