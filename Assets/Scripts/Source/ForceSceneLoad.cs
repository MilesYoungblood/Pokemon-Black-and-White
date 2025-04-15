using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class ForceSceneLoad : MonoBehaviour
    {
        [SerializeField] private string id;

        private void Awake()
        {
            for (var i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == id && !scene.isLoaded)
                {
                    SceneManager.LoadScene(id);
                }
            }

            Destroy(gameObject);
        }
    }
}
