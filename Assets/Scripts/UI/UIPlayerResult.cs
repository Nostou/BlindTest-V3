using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
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
        public string Text;
        public Color Color;
    }
    
    public class UIPlayerResult : MonoBehaviour, IComparable<UIPlayerResult>
    {
        public BTPlayer BTPlayer { get; set; }
        public bool UseFreeze => toggleFreeze.isOn;
        public ResultType ResultType => resultType;

        [SerializeField] private TMP_Text txtRank;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtScore;
        [SerializeField] private TMP_Text txtStreak;
        [SerializeField] private Button btnResult;
        [SerializeField] private TMP_Text txtResult;
        [SerializeField] private Toggle toggleFreeze;
        [SerializeField] private TMP_Text txtFreeze;
        [SerializeField] private SerializedDictionary<ResultType, ColorText> colorTexts;

        private ResultType resultType;
        private bool isAuthor;

        private void Start()
        {
            btnResult.onClick.AddListener(() =>
            {
                resultType++;
                if ((int)resultType == colorTexts.Count) resultType = 0;
                OnResultChanged();
            });
        }

        public void InitUI()
        {
            resultType = ResultType.NotFound;
            isAuthor = false;
            
            txtRank.text = $"{transform.GetSiblingIndex() + 1}.";
            txtName.text = BTPlayer.Name;
            
            txtScore.text = $"{BTPlayer.Score}";
            txtScore.color = Color.white;
            
            txtStreak.text = $"{BTPlayer.Streak}";
            
            txtResult.text = colorTexts[resultType].Text;
            txtResult.color = colorTexts[resultType].Color;
            txtResult.GetComponent<UIHoverScale>().SetHoverable(true);
            
            txtFreeze.text = $"({BTPlayer.StreakFreeze})";
            toggleFreeze.interactable = BTPlayer.Streak > 0 && BTPlayer.StreakFreeze > 0;
            toggleFreeze.isOn = false;
        }
        
        public void SetAuthor()
        {
            isAuthor = true;
            txtName.color = Color.cyan;
            txtResult.text = "Author";
            txtResult.color = Color.cyan;
            txtResult.GetComponent<UIHoverScale>().SetHoverable(false);
            toggleFreeze.interactable = false;
        }

        private void OnResultChanged()
        {
            int futureScore = BTPlayer.GetFutureAddScore(resultType);
            txtScore.text = futureScore == 0 ? $"{BTPlayer.Score}" : $"{BTPlayer.Score} (+{futureScore})";
            txtScore.color = futureScore == 0 ? Color.white : colorTexts[resultType].Color;
            
            txtResult.text = colorTexts[resultType].Text;
            txtResult.color = colorTexts[resultType].Color;
            
            toggleFreeze.interactable = BTPlayer.Streak > 0 && BTPlayer.StreakFreeze > 0 && resultType == 0 && !isAuthor;
            if (resultType != ResultType.NotFound) toggleFreeze.isOn = false;
        }

        public void FinalUI()
        {
            txtRank.text = $"{transform.GetSiblingIndex() + 1}.";
            
            txtName.color = Color.white;
            
            txtScore.text = $"{BTPlayer.Score}";
            txtScore.color = Color.white;
            
            txtStreak.text = $"{BTPlayer.Streak}";

            txtResult.text = "-";
            txtResult.color = Color.white;
            txtResult.GetComponent<UIHoverScale>().SetHoverable(false);
            
            txtFreeze.text = $"({BTPlayer.StreakFreeze})";
            toggleFreeze.interactable = false;
            toggleFreeze.isOn = false;
        }

        public int CompareTo(UIPlayerResult other)
        {
            int scoreComparison = other.BTPlayer.Score.CompareTo(BTPlayer.Score);
            if (scoreComparison != 0) return scoreComparison;
            
            int streakComparison = other.BTPlayer.Streak.CompareTo(BTPlayer.Streak);
            if (streakComparison != 0) return streakComparison;
            
            return String.Compare(BTPlayer.Name, other.BTPlayer.Name, StringComparison.Ordinal);
        }
    }
}
