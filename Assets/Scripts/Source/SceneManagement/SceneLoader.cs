using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public class SceneLoader : MonoBehaviour
    {
        private bool _working;

        private readonly Queue<SceneInfo> _sceneRequests = new();

        private Queue<AsyncOperation> _operations;

        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                enabled = false;
            }
        }

        private IEnumerator TransitionScene()
        {
            _working = true;
            _operations = _sceneRequests.Peek().TransitionScene();

            while (_operations.Any())
            {
                yield return new WaitUntil(IsDonePlaying);
                _operations.Dequeue();
            }

            _sceneRequests.Dequeue();
            if (_sceneRequests.Any())
            {
                yield return TransitionScene();
            }
            else
            {
                _working = false;
            }
        }

        private bool IsDonePlaying()
        {
            return _operations.Peek().isDone;
        }

        public void RequestSceneChange(SceneInfo scene)
        {
            _sceneRequests.Enqueue(scene);
            if (!_working)
            {
                StartCoroutine(TransitionScene());
            }
        }
    }
}
