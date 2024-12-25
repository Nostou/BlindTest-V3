using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuResult : MonoBehaviour
    {
        [SerializeField] private ButtonLongPressListener btnNext;
        [SerializeField] private Slider sliderVolume;
        
        private void OnEnable()
        {
            sliderVolume.value = 20;
        }

        private void Awake()
        {
            btnNext.OnLongPress += () =>
            {
                UIManager.Instance.LoadMenu(MenuType.Song);
                AudioManager.Instance.NextSong();
            };
            sliderVolume.onValueChanged.AddListener(AudioManager.Instance.SetVolume);
        }
    }
}