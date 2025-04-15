using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class SceneInfo : MonoBehaviour
    {
        [SerializeField] private SceneInfo[] connectedScenes;

        [SerializeField] private AudioClip music;

        private bool _isLoaded;

        private SavableEntity[] _savableEntities;

        public AudioClip Music => music;

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

            GameController.Instance.CurrentScene = this;

            AudioManager.Instance.RequestSong(music);

            // it is impossible to unload non-adjacent scenes if there is no previous scene
            if (!GameController.Instance.PreviousScene)
            {
                return operations;
            }

            // unload non-adjacent scenes
            foreach (var scene in GameController.Instance.PreviousScene.connectedScenes.Where(IsConnected))
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
            if (operation is not null)
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
            if (operation is not null)
            {
                operation.completed += OnLoaded;
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
                operation.completed += Unload;
            }

            return operation;
        }

        private void OnLoaded(AsyncOperation _)
        {
            _isLoaded = true;
            _savableEntities = GetSavableEntitiesInScene();
            SavingSystem.RestoreEntityStates(_savableEntities);
        }

        private void Unload(AsyncOperation _)
        {
            _isLoaded = false;
        }

        private SavableEntity[] GetSavableEntitiesInScene()
        {
            return FindObjectsByType<SavableEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(SceneExists).ToArray();
        }

        private bool IsConnected(SceneInfo scene)
        {
            return !connectedScenes.Contains(scene) && scene != this;
        }

        private bool SceneExists(SavableEntity entity)
        {
            return entity.gameObject.scene == SceneManager.GetSceneByName(gameObject.name);
        }
    }
}
