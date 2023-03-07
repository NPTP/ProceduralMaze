using Tools;
using UnityEngine;

namespace Audio
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource oneShotAudioSource;

        private void OnValidate()
        {
            if (musicAudioSource != null)
            {
                musicAudioSource.mute = false;
                musicAudioSource.bypassEffects = true;
                musicAudioSource.bypassListenerEffects = true;
                musicAudioSource.bypassReverbZones = true;
                musicAudioSource.playOnAwake = false;
                musicAudioSource.loop = true;

                musicAudioSource.volume = 0.25f;
            }
            
            if (oneShotAudioSource != null)
            {
                oneShotAudioSource.mute = false;
                oneShotAudioSource.bypassEffects = false;
                oneShotAudioSource.bypassListenerEffects = false;
                oneShotAudioSource.bypassReverbZones = false;
                oneShotAudioSource.playOnAwake = false;
                oneShotAudioSource.loop = false;
                
                oneShotAudioSource.volume = 1;
            }
        }

        private void Start()
        {
            musicAudioSource.Play();
        }

        public static void PlayOneShot(AudioClip clip)
        {
            if (clip != null && Instance.oneShotAudioSource != null)
            {
                Instance.oneShotAudioSource.PlayOneShot(clip);
            }
        }
    }
}