using System;
using System.Collections.Generic;
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
        
        [Header("Spectrum")]
        [SerializeField] private int nbBars = 64;
        [SerializeField] private float radius = 5f;
        [SerializeField] private float barWidth = 100;
        [SerializeField] private float barHeight = 500;
        [SerializeField] private float spectrumScale = 100;
        [SerializeField] private int indexOffset = 10;
        [SerializeField, Min(0)] private float fillDuration = 0.05f;
        [SerializeField] private UISpectrumImage barPrefab;
        [SerializeField] private Transform barParent;
        [SerializeField] private List<UISpectrumImage> barList = new List<UISpectrumImage>();
        
        private float[] spectrum = new float[512];
        private bool isPlaying;
        
        private void OnEnable()
        {
            sliderVolume.value = 50;
            txtMusicCount.text = $"{AudioManager.Instance.MusicIndex + 1}/{AudioManager.Instance.MusicCount}";
            UpdateTxtTimer(GameManager.Instance.GetCurrentSettings().Time);
            fillCircle.DOKill();
            fillCircle.fillAmount = 0;
        }

        private void OnDisable()
        {
            foreach (UISpectrumImage bar in barList)
            {
                bar.DOComplete();
            }
        }

        private void Start()
        {
            btnNext.OnLongPress += () =>
            {
                UIManager.Instance.LoadMenu(MenuType.Result);
                AudioManager.Instance.ToResult();
            };
            sliderVolume.onValueChanged.AddListener(AudioManager.Instance.SetVolume);

            AudioManager.Instance.OnMusicStarted += () =>
            {
                isPlaying = true;
                FillCircle();
            };
            AudioManager.Instance.OnMusicTick += UpdateTxtTimer;
            AudioManager.Instance.OnMusicEnded += () =>
            {
                foreach (UISpectrumImage bar in barList)
                {
                    bar.Fill(0, fillDuration);
                }
                isPlaying = false;
            };
        }

        private void Update()
        {
            if (isPlaying) UpdateSpectrum();
        }

        [ContextMenu("CreateBars")]
        private void CreateBars()
        {
            barList.Clear();
            while (barParent.childCount > 0)
            {
                DestroyImmediate(barParent.GetChild(0).gameObject);
            }

            for (int i = 0; i < nbBars; i++)
            {
                float angle = i * Mathf.PI * 2 / nbBars;
                UISpectrumImage go = Instantiate(barPrefab, barParent);
                go.transform.localPosition = new Vector3(Mathf.Sin(angle) * radius, Mathf.Cos(angle) * radius, 0);
                go.transform.localRotation = Quaternion.Euler(0, 0, -angle * Mathf.Rad2Deg);
                go.SetDimensions(barWidth, barHeight);
                barList.Add(go);
            }
        }

        private void UpdateSpectrum()
        {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
            float volume = Mathf.Clamp(AudioManager.Instance.Volume, 0.01f, 1.0f);

            for (int i = 0; i < nbBars; i++)
            {
                int index = i + indexOffset;
                float value = spectrum[index] / volume * spectrumScale;
                barList[i].Fill(value, fillDuration);
            }
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
