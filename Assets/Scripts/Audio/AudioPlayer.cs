using Tools;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource oneShotAudioSource;

        private void OnValidate()
        {
            if (musicAudioSource == null || oneShotAudioSource == null)
            {
                AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
                
                if (audioSources.Length == 1)
                {
                    AudioSource secondAudioSource = gameObject.AddComponent<AudioSource>();
                    audioSources = new[] { audioSources[0], secondAudioSource };
                }
                
                musicAudioSource = audioSources[0];
                oneShotAudioSource = audioSources[1];
            }
        }

        public static void PlayOneShot(AudioClip clip)
        {
            if (clip != null)
            {
                Instance.oneShotAudioSource.PlayOneShot(clip);
            }
        }
    }
}