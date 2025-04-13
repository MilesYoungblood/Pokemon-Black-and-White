using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Source
{
    public class SceneInfo : MonoBehaviour
    {
        [SerializeField] private SceneInfo[] connectedScenes;

        [SerializeField] private AudioClip music;

        private bool _isLoaded;

        private SavableEntity[] _savableEntities;

        public Location Location { get; private set; }

        public AudioClip Music => music;

        private void Awake()
        {
            Location = GetComponent<Location>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // only the player is allowed to trigger the scene swapping
            if (other.CompareTag("Player"))
            {
                SceneLoader.Instance.RequestSceneChange(this);
            }
        }

        public Queue<AsyncOperation> TransitionScene()
        {
            Queue<AsyncOperation> operations = new();

            // load the current scene and connected scenes
            TryAddOperation(operations, LoadScene());

            foreach (var scene in connectedScenes)
            {
                TryAddOperation(operations, scene.LoadScene());
            }

            GameController.Instance.TransitionScene(this);

            AudioManager.Instance.RequestSong(music);

            // it is impossible to unload non-adjacent scenes if there is no previous scene
            if (!GameController.Instance.PreviousScene)
            {
                return operations;
            }

            // unload non-adjacent scenes
            var previousScenes = GameController.Instance.PreviousScene.connectedScenes;
            foreach (var scene in previousScenes.Where(scene => !connectedScenes.Contains(scene) && scene != this))
            {
                TryAddOperation(operations, scene.UnloadScene());
            }

            if (connectedScenes.Contains(GameController.Instance.PreviousScene))
            {
                return operations;
            }

            TryAddOperation(operations, GameController.Instance.PreviousScene.UnloadScene());

            return operations;
        }

        private static void TryAddOperation(Queue<AsyncOperation> operations, AsyncOperation operation)
        {
            if (operation != null)
            {
                operations.Enqueue(operation);
            }
        }

        private AsyncOperation LoadScene()
        {
            if (_isLoaded)
            {
                return null;
            }

            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            if (operation != null)
            {
                operation.completed += _ =>
                {
                    _isLoaded = true;
                    _savableEntities = GetSavableEntitiesInScene();
                    SavingSystem.RestoreEntityStates(_savableEntities);
                };
            }

            return operation;
        }

        [CanBeNull]
        private AsyncOperation UnloadScene()
        {
            if (!_isLoaded)
            {
                return null;
            }

            SavingSystem.CaptureEntityStates(_savableEntities);
            var operation = SceneManager.UnloadSceneAsync(gameObject.name);
            if (operation != null)
            {
                operation.completed += _ =>
                {
                    _isLoaded = false;
                };
            }

            return operation;
        }

        private SavableEntity[] GetSavableEntitiesInScene()
        {
            var currentScene = SceneManager.GetSceneByName(gameObject.name);
            var list1 = FindObjectsByType<SavableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            return list1.Where(entity => entity.gameObject.scene == currentScene).ToArray();
        }
    }
}
