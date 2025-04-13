using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Source.UI
{
    [DisallowMultipleComponent]
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private RectTransform hp;

        [SerializeField] private Image image;

        public static event Action OnHpReturned;

        public static event Action OnLowHp;

        public void SetHp(float health)
        {
            hp.localScale = new Vector3(health, 1.0f);
            image.color = hp.localScale.x switch
            {
                > 0.5f => Color.green,
                > 0.1f => Color.yellow,
                _ => Color.red
            };
        }

        public IEnumerator SetHpSmooth(float health)
        {
            var currentHp = hp.localScale.x;
            var changeAmount = currentHp - health;

            while (currentHp - health > Mathf.Epsilon)
            {
                currentHp -= changeAmount * Time.deltaTime;
                SetHp(currentHp);
                yield return null;
            }

            SetHp(health);

            switch (hp.localScale.x)
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
