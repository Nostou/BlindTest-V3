using System.Collections.Generic;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace UI
{
    public class UIAudioSpectrum : MonoBehaviour
    {
        [SerializeField] private int nbBars = 64;
        [SerializeField] private float radius = 5f;
        [SerializeField] private float barWidth = 100;
        [SerializeField] private float barHeight = 500;
        [SerializeField] private float spectrumScale = 100;
        [SerializeField] private int indexOffset = 10;
        [SerializeField, Min(0)] private float fillDuration = 0.05f;
        [SerializeField] private UISpectrumImage barPrefab;
        [SerializeField] private List<UISpectrumImage> barList = new List<UISpectrumImage>();
        
        private float[] spectrum = new float[512];
        private bool isPlaying;
        
        private void OnDisable()
        {
            ResetFills(true);
        }

        private void Start()
        {
            AudioManager.Instance.OnMusicStarted += () => isPlaying = true;
            AudioManager.Instance.OnMusicEnded += () =>
            {
                isPlaying = false;
                ResetFills(false);
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
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            for (int i = 0; i < nbBars; i++)
            {
                float angle = i * Mathf.PI * 2 / nbBars;
                UISpectrumImage go = Instantiate(barPrefab, transform);
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

        private void ResetFills(bool isHardReset)
        {
            foreach (UISpectrumImage bar in barList)
            {
                if (isHardReset) bar.Fill(0,0);
                else bar.Fill(0, fillDuration);
            }
        }
    }
}
