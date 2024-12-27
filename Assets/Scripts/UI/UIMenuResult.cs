using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuResult : MonoBehaviour
    {
        [SerializeField] private ButtonLongPressListener btnNext;
        [SerializeField] private ButtonLongPressListener btnConfirm;
        [SerializeField] private ButtonLongPressListener btnRewind;
        [SerializeField] private Slider sliderVolume;
        [SerializeField] private UIRankings uiRankings;
        
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
            
            btnConfirm.OnLongPress += ConfirmResult;
            btnRewind.OnLongPress += RewindResult;
        }

        private void ConfirmResult()
        {
            uiRankings.ComputeRankings();
        }

        private void RewindResult()
        {
            
        }
    }
}