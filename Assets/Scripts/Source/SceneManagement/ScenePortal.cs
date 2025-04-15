using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class ScenePortal : MonoBehaviour
    {
        [SerializeField] private string location;

        [SerializeField] private Vector2 spawnPoint;

        [SerializeField] private bool fade;

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController>(out var playerController))
            {
                yield break;
            }

            yield return new WaitUntil(playerController.IsCenteredOnTile);

            DontDestroyOnLoad(gameObject);

            GameController.Instance.TogglePause();
            if (fade)
            {
                yield return GameController.Instance.Fader.FadeIn(0.5f);
            }

            yield return SceneManager.LoadSceneAsync(location);
            playerController.transform.position = new Vector3(spawnPoint.x, spawnPoint.y);

            if (fade)
            {
                yield return GameController.Instance.Fader.FadeOut(0.5f);
            }

            GameController.Instance.TogglePause();
            Destroy(gameObject);
        }
    }
}
