using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class HPBar : MonoBehaviour
    {
        [SerializeField] private Image image;

        public float HP
        {
            set
            {
                image.transform.localScale = new Vector3(Mathf.Clamp01(value), 1.0f, 1.0f);
                image.color = image.transform.localScale.x switch
                {
                    > 0.5f => Color.green,
                    > 0.1f => Color.yellow,
                    _ => Color.red
                };
            }
        }

        public IEnumerator SetHPSmooth(float targetHealth, float duration = 0.5f)
        {
            var startHealth = image.transform.localScale.x;

            for (var elapsed = 0.0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                HP = Mathf.Lerp(startHealth, targetHealth, elapsed / duration);
                yield return null;
            }

            HP = targetHealth;
        }
    }
}
