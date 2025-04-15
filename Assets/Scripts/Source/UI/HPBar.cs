using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class HPBar : MonoBehaviour
    {
        [SerializeField] private RectTransform hp;

        [SerializeField] private Image image;

        public float HP
        {
            set
            {
                hp.localScale = new Vector3(value, 1.0f, 1.0f);
                image.color = hp.localScale.x switch
                {
                    > 0.5f => Color.green,
                    > 0.1f => Color.yellow,
                    _ => Color.red
                };
            }
        }

        public static event Action OnHpReturned;

        public static event Action OnLowHp;

        public IEnumerator SetHPSmooth(float targetHealth, float duration = 0.5f)
        {
            var startHealth = hp.localScale.x;

            for (var elapsed = 0.0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                HP = Mathf.Lerp(startHealth, targetHealth, elapsed / duration);
                yield return null;
            }

            HP = targetHealth; // Ensure final precision

            switch (targetHealth)
            {
                case > 0.1f:
                    OnHpReturned?.Invoke();
                    break;
                default:
                    OnLowHp?.Invoke();
                    break;
            }
        }
    }
}
