using System.Collections.Generic;
using Managers;
using ScriptableObjects;
using UnityEngine;

namespace UI
{
    public class UIRankings : MonoBehaviour
    {
        [SerializeField] private UIPlayerResult uiPlayerResultPrefab;
        [SerializeField] private Transform uiPlayerResultParent;

        private List<UIPlayerResult> gridResult = new List<UIPlayerResult>();
        //private List<BTPlayer> rewindPlayers = new List<BTPlayer>();

        private void Awake()
        {
            CreateRankings();
        }
        
        private void CreateRankings()
        {
            foreach (BTPlayer btPlayer in GameManager.Instance.Players)
            {
                UIPlayerResult uiPlayerResult = Instantiate(uiPlayerResultPrefab, uiPlayerResultParent);
                uiPlayerResult.BTPlayer = btPlayer;
                uiPlayerResult.UpdateUI();
                gridResult.Add(uiPlayerResult);
            }
        }

        public void ComputeRankings()
        {
            BTSettingsSO settings = GameManager.Instance.GetCurrentSettings();
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                BTPlayer btPlayer = uiPlayerResult.BTPlayer;

                int addScore = 0;
                if (uiPlayerResult.ResultType == ResultType.NotFound && !uiPlayerResult.UseFreeze) btPlayer.Streak = 0;
                else if (uiPlayerResult.ResultType == ResultType.Golden) addScore = settings.ScoreGolden;
                else if (uiPlayerResult.ResultType == ResultType.First) addScore = settings.ScoreFirst;
                else if (uiPlayerResult.ResultType == ResultType.Second) addScore = settings.ScoreSecond;
                
                if (settings.StreakEnabled) addScore *= btPlayer.Streak * settings.StreakValue / 100;
                btPlayer.Score += addScore;

                if (uiPlayerResult.ResultType != ResultType.NotFound)
                {
                    btPlayer.Streak++;
                    btPlayer.Streak = Mathf.Min(btPlayer.Streak, settings.StreakMax);
                }
            }
            
            gridResult.Sort();
            for (int i = 0; i < gridResult.Count; i++)
            {
                gridResult[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
