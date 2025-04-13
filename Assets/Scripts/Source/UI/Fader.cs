using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    [RequireComponent(typeof(Image))]
    public class Fader : MonoBehaviour
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetRGB(Color color)
        {
            _image.color = new Color(color.r, color.g, color.b, _image.color.a);
        }

        public void SetAlpha(float a)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Clamp01(a));
        }

        public IEnumerator FadeIn(float time)
        {
            yield return _image.DOFade(1.0f, time).WaitForCompletion();
        }

        public IEnumerator FadeOut(float time)
        {
            yield return _image.DOFade(0.0f, time).WaitForCompletion();
        }
    }
}
