using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio
{
    public class ButtonAudio : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioClip clickSound;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioPlayer.PlayOneShot(clickSound);
        }
    }
}