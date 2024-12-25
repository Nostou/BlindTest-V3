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
            sliderVolume.value = 15;
        }

        private void Start()
        {
            btnNext.OnLongPress += () =>
            {
                UIManager.Instance.LoadMenu(MenuType.Music);
                AudioManager.Instance.NextMusic();
            };
            sliderVolume.onValueChanged.AddListener(AudioManager.Instance.SetVolume);
        }
    }
}