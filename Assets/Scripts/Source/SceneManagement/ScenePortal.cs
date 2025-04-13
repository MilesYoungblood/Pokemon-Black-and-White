using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Source
{
    public class ScenePortal : MonoBehaviour, ITriggerable
    {
        [SerializeField] private string location;

        [SerializeField] private Vector2 spawnPoint;

        [SerializeField] private bool fade;

        public void OnTrigger(PlayerController playerController)
        {
            StartCoroutine(SwitchScene(playerController));
        }

        private IEnumerator SwitchScene(Component playerController)
        {
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
