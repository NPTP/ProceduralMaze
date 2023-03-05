using Tools;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource oneShotAudioSource;
        
        public static void PlayOneShot(AudioClip clip)
        {
            if (clip != null && Instance.oneShotAudioSource != null)
            {
                Instance.oneShotAudioSource.PlayOneShot(clip);
            }
        }
    }
}