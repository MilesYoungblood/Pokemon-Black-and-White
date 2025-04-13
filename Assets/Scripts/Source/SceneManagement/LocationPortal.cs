using System.Collections;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class LocationPortal : MonoBehaviour, ITriggerable
    {
        [SerializeField] private Vector2 spawnPoint;

        [SerializeField] private bool fade;

        public void OnTrigger(PlayerController playerController)
        {
            StartCoroutine(Relocate(playerController));
        }

        private IEnumerator Relocate(Component playerController)
        {
            GameController.Instance.TogglePause();
            if (fade)
            {
                yield return GameController.Instance.Fader.FadeIn(0.5f);
            }

            playerController.transform.position = spawnPoint;

            if (fade)
            {
                yield return GameController.Instance.Fader.FadeOut(0.5f);
            }

            GameController.Instance.TogglePause();
        }
    }
}
