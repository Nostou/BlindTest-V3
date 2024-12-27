using System;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public enum ResultType
    {
        NotFound,
        Golden,
        First,
        Second
    }
    
    [Serializable]
    public class ColorText
    {
        public ResultType ResultType;
        public string Text;
        public Color Color;
    }
    
    public class UIPlayerResult : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IComparable<UIPlayerResult>
    {
        public BTPlayer BTPlayer { get; set; }
        public bool UseFreeze => toggleFreeze.isOn;
        public ResultType ResultType => colorTexts[index].ResultType;

        [SerializeField] private TMP_Text txtRank;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtScore;
        [SerializeField] private TMP_Text txtStreak;
        [SerializeField] private Button btnResult;
        [SerializeField] private TMP_Text txtResult;
        [SerializeField] private Toggle toggleFreeze;
        [SerializeField] private TMP_Text txtFreeze;
        [SerializeField] private List<ColorText> colorTexts;

        private int index;

        private void OnEnable()
        {
            index = 0;
            toggleFreeze.isOn = false;
            UpdateUI();
        }

        private void Start()
        {
            btnResult.onClick.AddListener(() =>
            {
                index++;
                if (index == colorTexts.Count) index = 0;
                UpdateUI();
            });
        }

        public void UpdateUI()
        {
            if (BTPlayer == null) return;
            
            txtRank.text = $"{transform.GetSiblingIndex() + 1}.";
            txtName.text = BTPlayer.Name;

            int futureScore = BTPlayer.GetFutureAddScore(colorTexts[index].ResultType);
            txtScore.text = futureScore == 0 ? $"{BTPlayer.Score}" : $"{BTPlayer.Score} (+{futureScore})";
            txtScore.color = futureScore == 0 ? Color.white : colorTexts[index].Color;
            
            txtStreak.text = $"{BTPlayer.Streak}";
            
            txtResult.text = colorTexts[index].Text;
            txtResult.color = colorTexts[index].Color;
            
            txtFreeze.text = $"({BTPlayer.StreakFreeze})";
            toggleFreeze.interactable = BTPlayer.StreakFreeze > 0 && index == 0;
            if (colorTexts[index].ResultType != ResultType.NotFound) toggleFreeze.isOn = false;
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

        public int CompareTo(UIPlayerResult other)
        {
            int scoreComparison = BTPlayer.Score.CompareTo(other.BTPlayer.Score);
            if (scoreComparison != 0) return scoreComparison;
            
            int streakComparison = BTPlayer.Streak.CompareTo(other.BTPlayer.Streak);
            if (streakComparison != 0) return streakComparison;
            
            return String.Compare(BTPlayer.Name, other.BTPlayer.Name, StringComparison.Ordinal);
        }
    }
}
