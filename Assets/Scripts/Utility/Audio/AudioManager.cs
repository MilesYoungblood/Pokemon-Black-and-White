using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scripts.Utility
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private int poolSize;

        [SerializeField] private AudioSource musicPlayer;

        [SerializeField] private Transform sfx;

        [SerializeField] private GameObject soundEffect;

        private readonly Dictionary<string, AsyncOperationHandle<AudioClip>> _loadedClips = new();

        private readonly Dictionary<string, AudioClip> _sfxClips = new();

        private readonly List<AudioSource> _sfxPool = new();

        private readonly Queue<AudioClip> _songRequests = new();

        private bool _working;

        public bool IsLoaded { get; private set; }

        public static AudioManager Instance { get; private set; }

        public static event Action OnSoundFinishedPlaying;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject); // Ensure Singleton
            }
            else
            {
                Instance = this;

                InitializeSFXPool();
            }
        }

        private async void Start()
        {
            var stopwatch = Stopwatch.StartNew();
            var loadTasks = new List<Task<AudioClip>>();

            const string path = "Assets/Audio/SFX/";
            foreach (var file in Directory.GetFiles(path, "*.wav", SearchOption.AllDirectories))
            {
                var relativePath = file[file.IndexOf(path, StringComparison.Ordinal)..]
                    .Replace(Path.PathSeparator, '/');
                var handle = Addressables.LoadAssetAsync<AudioClip>(relativePath);
                loadTasks.Add(handle.Task);

                handle.Completed += OnCompletion;
            }

            await Task.WhenAll(loadTasks);

            IsLoaded = true;
            stopwatch.Stop();
            print($"Loaded all sound effects in {stopwatch.Elapsed.ToString()}.");
        }

        private void InitializeSFXPool()
        {
            for (var i = 0; i < poolSize; ++i)
            {
                var newSource = Instantiate(soundEffect, sfx.transform);
                newSource.gameObject.SetActive(false);
                if (newSource.TryGetComponent<AudioSource>(out var audioSource))
                {
                    _sfxPool.Add(audioSource);
                }
                else
                {
                    throw new Exception($"Error retrieving {nameof(AudioSource)} component.");
                }
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var source in _sfxPool.Where(source => !source.isPlaying))
            {
                return source;
            }

            if (Instantiate(soundEffect, sfx.transform).TryGetComponent<AudioSource>(out var audioSource))
            {
                _sfxPool.Add(audioSource);
            }
            else
            {
                throw new Exception($"Error retrieving {nameof(AudioSource)} component.");
            }

            return audioSource;
        }

        private void OnCompletion(AsyncOperationHandle<AudioClip> handle)
        {
            if (handle.Status is AsyncOperationStatus.Succeeded && handle.Result)
            {
                _sfxClips[handle.Result.name] = handle.Result;
                _loadedClips[handle.Result.name] = handle; // Cache handle
            }
            else
            {
                print($"Error loading sound effect: {handle.OperationException}");
            }
        }

        public AsyncOperationHandle<AudioClip> LoadClipAsync(string clipName)
        {
            if (_loadedClips.TryGetValue(clipName, out var existingHandle))
            {
                return existingHandle;
            }

            _loadedClips[clipName] = Addressables.LoadAssetAsync<AudioClip>($"Assets/Audio/Music/{clipName}.mp3");
            return _loadedClips[clipName];
        }

        public void UnloadClip(string clipName)
        {
            if (!_loadedClips.TryGetValue(clipName, out var handle))
            {
                return;
            }

            Addressables.Release(handle);
            _loadedClips.Remove(clipName);
        }

        private async Task HandleSongRequest()
        {
            _working = true;

            while (_songRequests.Any())
            {
                await WaitForLoad();
                await FadeMusic();
                PlayMusic(_songRequests.Dequeue(), true);
            }

            _working = false;
        }

        private async Task WaitForLoad()
        {
            while (!IsLoaded)
            {
                await Task.Yield();
            }
        }

        public async Task RequestSong(AudioClip clip)
        {
            _songRequests.Enqueue(clip);
            if (!_working)
            {
                await HandleSongRequest();
            }
        }

        private async Task FadeMusic()
        {
            if (!musicPlayer.clip)
            {
                return;
            }

            var originalVolume = musicPlayer.volume;
            await musicPlayer.DOFade(0, 1).AsyncWaitForCompletion();
            musicPlayer.volume = originalVolume;
        }

        public void PlayMusic(AudioClip clip, bool loop)
        {
            if (musicPlayer.clip && clip.name == musicPlayer.clip.name)
            {
                return;
            }

            musicPlayer.clip = clip;
            musicPlayer.loop = loop;
            musicPlayer.Play();
        }

        public async Task PlayMusicAsync(string musicName, bool loop)
        {
            var operationHandle = LoadClipAsync(musicName);
            await operationHandle.Task;

            if (operationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                PlayMusic(operationHandle.Result, loop);
            }
            else
            {
                print($"Failed to load music: {musicName}");
            }
        }

        public async Task PlayFanfare(string musicName)
        {
            var previousClip = musicPlayer.clip;
            var previousLoop = musicPlayer.loop;
            var previousTime = musicPlayer.time;

            StopMusic();
            await PlayMusicAsync(musicName, false);
            await WaitForMusicToEnd();

            ContinueMusic(previousClip, previousLoop, previousTime);
        }

        private async Task WaitForMusicToEnd()
        {
            while (musicPlayer.isPlaying)
            {
                await Task.Yield();
            }
        }

        private void ContinueMusic(AudioClip previousClip, bool previousLoop, float previousTime)
        {
            musicPlayer.clip = previousClip;
            musicPlayer.loop = previousLoop;
            musicPlayer.time = previousTime;

            var previousVolume = musicPlayer.volume;
            musicPlayer.volume = 0;

            musicPlayer.Play();
            musicPlayer.DOFade(previousVolume, 0.5f);
        }

        public void StopMusic()
        {
            musicPlayer.Stop();
        }

        public AudioSource PlaySound(AudioClip clip)
        {
            var source = GetAvailableSFXSource();
            source.clip = clip;
            source.gameObject.SetActive(true);
            source.Play();
            return source;
        }

        public AudioSource PlaySound(string soundName)
        {
            if (!IsLoaded || !_sfxClips.TryGetValue(soundName, out var clip))
            {
                return new AudioSource();
            }

            return PlaySound(clip);
        }

        public async Task<AudioSource> PlaySoundAsync(AudioClip clip)
        {
            await WaitForLoad();
            return PlaySound(clip);
        }

        private async Task<AudioSource> PlaySoundAsync(string soundName)
        {
            await WaitForLoad();
            return PlaySound(soundName);
        }

        public async Task PlaySoundToCompletion(AudioClip clip)
        {
            var source = PlaySoundAsync(clip);
            await source;
            while (source.Result.isPlaying)
            {
                await Task.Yield();
            }

            OnSoundFinishedPlaying?.Invoke();
        }

        public async Task PlaySoundToCompletion(string soundName)
        {
            var source = PlaySoundAsync(soundName);
            await source;
            while (source.Result.isPlaying)
            {
                await Task.Yield();
            }

            OnSoundFinishedPlaying?.Invoke();
        }
    }
}
