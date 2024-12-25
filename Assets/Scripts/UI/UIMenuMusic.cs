using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuMusic : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ButtonLongPressListener btnNext;
        [SerializeField] private Slider sliderVolume;
        [SerializeField] private TMP_Text txtMusicCount;
        [SerializeField] private Image fillCircle;
        [SerializeField] private TMP_Text txtTimer;
        
        private void OnEnable()
        {
            sliderVolume.value = 50;
            txtMusicCount.text = $"{AudioManager.Instance.MusicIndex + 1}/{AudioManager.Instance.MusicCount}";
            UpdateTxtTimer(GameManager.Instance.GetCurrentSettings().Time);
            fillCircle.DOKill();
            fillCircle.fillAmount = 0;
        }

        private void Start()
        {
            btnNext.OnLongPress += () =>
            {
                UIManager.Instance.LoadMenu(MenuType.Result);
                AudioManager.Instance.ToResult();
            };
            sliderVolume.onValueChanged.AddListener(AudioManager.Instance.SetVolume);

            AudioManager.Instance.OnMusicStarted += FillCircle;
            AudioManager.Instance.OnMusicTick += UpdateTxtTimer;
        }

        private void FillCircle()
        {
            fillCircle.DOFillAmount(1, GameManager.Instance.GetCurrentSettings().Time).SetEase(Ease.Linear);
        }
        
        private void UpdateTxtTimer(int duration)
        {
            txtTimer.text = $"{duration}";
        }
    }
}
