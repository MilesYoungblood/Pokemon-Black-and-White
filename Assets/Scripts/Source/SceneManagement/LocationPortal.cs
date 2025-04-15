using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class LocationPortal : MonoBehaviour
    {
        [SerializeField] private Vector2 spawnPoint;

        [SerializeField] private bool fade;

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var playerController))
            {
                yield break;
            }

            yield return new WaitUntil(playerController.IsCenteredOnTile);

            GameController.Instance.TogglePause();
            if (fade)
            {
                yield return GameController.Instance.Fader.FadeIn(0.5f);
            }

            playerController.transform.position = spawnPoint - Vector2.up / 2.0f;

            if (fade)
            {
                yield return GameController.Instance.Fader.FadeOut(0.5f);
            }

            GameController.Instance.TogglePause();
        }
    }
}
