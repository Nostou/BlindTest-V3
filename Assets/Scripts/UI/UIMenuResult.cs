using Managers;
using TMPro;
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
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private UIRankings uiRankings;
        
        private void OnEnable()
        {
            sliderVolume.value = 15;
            btnConfirm.SetInteractable(true);
            btnNext.SetInteractable(false);
            
            AudioManager am = AudioManager.Instance;
            uiRankings.InitRankings(am.CurrentMusic.Author);
            txtTitle.text = $"{am.CurrentMusic.Title} ({am.MusicIndex+1}/{am.MusicCount})";
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
            btnConfirm.SetInteractable(false);
            
            bool endOfGame = AudioManager.Instance.MusicIndex == AudioManager.Instance.MusicCount - 1;
            btnNext.SetInteractable(!endOfGame);
        }

        private void RewindResult()
        {
            
        }
    }
}