using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio
{
    /// <summary>
    /// Place this MonoBehaviour on a button object to get the desired sound when clicking.
    /// </summary>
    public class ButtonAudio : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioClip clickSound;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioPlayer.PlayOneShot(clickSound);
        }
    }
}