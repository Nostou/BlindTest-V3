using System;
using AYellowpaper.SerializedCollections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
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

        private int maxStreakValue;

        private void Start()
        {
            maxStreakValue = GameManager.Instance.GetCurrentSettings().StreakMax;
            
            btnResult.onClick.AddListener(() =>
            {
                resultType++;
                if ((int)resultType == colorTexts.Count) resultType = 0;
                OnResultChanged();
            });
            
            toggleFreeze.onValueChanged.AddListener(OnStreakFroze);
        }

        public void InitUI()
        {
            resultType = ResultType.NotFound;
            isAuthor = false;
            
            txtRank.text = $"{transform.GetSiblingIndex() + 1}.";
            txtName.text = BTPlayer.Name;
            txtName.color = Color.white;
            txtScore.text = $"{BTPlayer.Score}";
            txtScore.color = Color.white;
            txtStreak.text = $"{BTPlayer.Streak}";
            UpdateResultUI(colorTexts[resultType]);
            toggleFreeze.interactable = BTPlayer.Streak > 0 && BTPlayer.StreakFreeze > 0;
            toggleFreeze.isOn = false;
            txtFreeze.text = $"({BTPlayer.StreakFreeze})";
            SetHover(true);
        }

        public void SetAuthor()
        {
            isAuthor = true;
            txtName.color = Color.cyan;
            txtResult.text = "Author";
            txtResult.color = Color.cyan;
            toggleFreeze.interactable = false;
            SetHover(false);
        }
        
        public void RewindUI(ResultType type, bool useFreeze)
        {
            resultType = type;
            OnResultChanged();
            toggleFreeze.isOn = useFreeze;
        }

        public void FinalUI()
        {
            txtRank.text = $"{transform.GetSiblingIndex() + 1}.";
            txtName.color = Color.white;
            txtScore.text = $"{BTPlayer.Score}";
            txtScore.color = Color.white;
            txtStreak.text = $"{BTPlayer.Streak}";
            UpdateResultUI(new ColorText { Text = "-", Color = Color.white });
            txtFreeze.text = $"({BTPlayer.StreakFreeze})";
            toggleFreeze.interactable = false;
            toggleFreeze.isOn = false;
            SetHover(false);
        }

        private void OnResultChanged()
        {
            int futureScore = BTPlayer.GetFutureAddScore(resultType);
            txtScore.text = futureScore == 0 ? $"{BTPlayer.Score}" : $"{BTPlayer.Score} (+{futureScore})";
            txtScore.color = futureScore == 0 ? Color.white : colorTexts[resultType].Color;

            int futureStreak = Mathf.Min(BTPlayer.Streak + (resultType != ResultType.NotFound ? 1 : 0), maxStreakValue);
            txtStreak.text = $"{futureStreak}";

            UpdateResultUI(colorTexts[resultType]);

            toggleFreeze.interactable = BTPlayer.Streak > 0 && BTPlayer.StreakFreeze > 0 && resultType == ResultType.NotFound && !isAuthor;
            if (resultType != ResultType.NotFound) toggleFreeze.isOn = false;
        }

        private void OnStreakFroze(bool newValue)
        {
            txtFreeze.text = $"({BTPlayer.StreakFreeze + (newValue ? -1 : 0)})";
        }

        private void UpdateResultUI(ColorText ct)
        {
            txtResult.text = ct.Text;
            txtResult.color = ct.Color;
        }

        private void SetHover(bool enable)
        {
            var hover = txtResult.GetComponent<UIHoverScale>();
            hover.SetHoverable(enable);
        }

        public int CompareTo(UIPlayerResult other)
        {
            int scoreComparison = other.BTPlayer.Score.CompareTo(BTPlayer.Score);
            if (scoreComparison != 0) return scoreComparison;

            int streakComparison = other.BTPlayer.Streak.CompareTo(BTPlayer.Streak);
            if (streakComparison != 0) return streakComparison;

            return string.Compare(BTPlayer.Name, other.BTPlayer.Name, StringComparison.Ordinal);
        }
    }
    
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
}
