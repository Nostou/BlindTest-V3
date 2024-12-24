using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIMenuSong : MonoBehaviour
    {
        [SerializeField] private int nbBars = 64;
        [SerializeField] private float radius = 5f;
        [SerializeField] private float barWidth = 100;
        [SerializeField] private float barHeight = 500;
        [SerializeField] private float spectrumScale = 100;
        [SerializeField] private int indexOffset = 10;
        [SerializeField] private UISpectrumImage barPrefab;
        [SerializeField] private Transform barParent;

        [SerializeField] private List<UISpectrumImage> barList = new List<UISpectrumImage>();
        private float[] spectrum = new float[1024];

        private void Update()
        {
            UpdateSpectrum();
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

            for (int i = 0; i < nbBars; i++)
            {
                int index = i + indexOffset;
                barList[i].Fill(spectrum[index]*spectrumScale, 0.05f);
            }
        }
    }
}
