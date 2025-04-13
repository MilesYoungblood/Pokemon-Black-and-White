using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void SetRGB(Color color)
        {
            image.color = new Color(color.r, color.g, color.b, image.color.a);
        }

        public void SetAlpha(float a)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(a));
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
