using System.Collections;
using UnityEngine;

namespace Scripts.Utility
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public IEnumerator Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();

            yield return !_audioSource.isPlaying;
            Destroy(gameObject);
        }
    }
}
