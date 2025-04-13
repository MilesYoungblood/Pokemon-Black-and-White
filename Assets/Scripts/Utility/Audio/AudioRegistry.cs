/*
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Utility
{
    public class AudioRegistry : MonoBehaviour
    {
        private readonly Dictionary<string, AudioClip> _sfxClips = new();

        public bool IsLoaded { get; private set; }

        private IEnumerator Start()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var operations = new List<AsyncOperationHandle<AudioClip>>();

            foreach (var file in Directory.GetFiles("Assets/Audio/SFX/", "*.wav", SearchOption.AllDirectories))
            {
                operations.Add(Addressables.LoadAssetAsync<AudioClip>(file.Replace('\\', '/')));
                operations.Last().Completed += HandleCompletion;
            }

            yield return new WaitUntil(() => operations.All(operation => operation.IsDone));

            foreach (var operation in operations) operation.Completed -= HandleCompletion;

            IsLoaded = true;

            stopwatch.Stop();
            print($"Loaded all sound effects in {stopwatch.Elapsed}.");

            yield break;

            void HandleCompletion(AsyncOperationHandle<AudioClip> handle)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    _sfxClips.Add(handle.Result.name, handle.Result);
                else
                    Addressables.LogError($"Error loading Addressable asset: \"{handle.OperationException}\".");
            }
        }
    }
}
*/