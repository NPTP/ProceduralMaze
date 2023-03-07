using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    /// <summary>
    /// Playable audio clip that uses a set of audio files with two playback modes
    /// </summary>
    [Serializable]
    public class AudioMultiClip
    {
        private enum PlaybackMode
        {
            Random = 0,
            RoundRobin
        }
        public bool IsEmpty => audioClips == null || audioClips.Length < 1;
        public AudioClip Clip => GetClip();

        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private PlaybackMode playbackMode;

        private int index;

        private AudioClip GetClip()
        {
            if (playbackMode is PlaybackMode.Random)
            {
                index = Random.Range(0, audioClips.Length);
            }

            AudioClip clip = audioClips[Mathf.Clamp(index, 0, audioClips.Length - 1)];
            
            if (playbackMode is PlaybackMode.RoundRobin)
            {
                index++;
                if (index >= audioClips.Length)
                {
                    index = 0;
                }
            }

            return clip;
        }
    }
}