using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace UI
{
    [Serializable]
    public class ColorText
    {
        public string Text;
        public Color Color;
    }
    
    public class UIPlayerResult : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button btnResult;
        [SerializeField] private TMP_Text txtResult;
        [SerializeField] private List<ColorText> colorTexts;

        private int index;

        private void OnEnable()
        {
            index = 0;
            UpdateText();
        }

        private void Start()
        {
            btnResult.onClick.AddListener(() =>
            {
                index++;
                if (index == colorTexts.Count) index = 0;
                UpdateText();
            });
        }

        private void UpdateText()
        {
            txtResult.text = colorTexts[index].Text;
            txtResult.color = colorTexts[index].Color;
        }

        private void TweenText(Vector3 targetScale)
        {
            txtResult.transform.DOKill();
            txtResult.transform.DOScale(targetScale, 0.2f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TweenText(Vector3.one * 1.2f);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            TweenText(Vector3.one);
        }
    }
}
