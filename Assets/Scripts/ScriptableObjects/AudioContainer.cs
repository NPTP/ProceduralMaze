using Audio;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// Container for the game's sounds.
    /// </summary>
    [CreateAssetMenu]
    public class AudioContainer : ScriptableObject
    {
        [Header("Music")]
        [SerializeField] private AudioClip music;
        public AudioClip Music => music;
        
        [Header("SFX")]
        [SerializeField] private AudioClip spinUp;
        public AudioClip SpinUp => spinUp;
    
        [SerializeField] private AudioClip wallsUp;
        public AudioClip WallsUp => wallsUp;
        
        [SerializeField] private AudioClip switchOn;
        public AudioClip SwitchOn => switchOn;
        
        [SerializeField] private AudioClip whoosh;
        public AudioClip Whoosh => whoosh;

        [SerializeField] private AudioClip winDing;
        public AudioClip WinDing => winDing;
                
        [SerializeField] private AudioClip lowBoom;
        public AudioClip LowBoom => lowBoom;
        
        [SerializeField] private AudioMultiClip uiTick;
        public AudioMultiClip UITick => uiTick;
    }
}