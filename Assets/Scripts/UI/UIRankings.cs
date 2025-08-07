using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<BTPlayer, RewindData> rewindResult;

        private BTPlayer currentAuthor;
        
        private void CreateRankings()
        {
            foreach (BTPlayer btPlayer in GameManager.Instance.Players)
            {
                UIPlayerResult uiPlayerResult = Instantiate(uiPlayerResultPrefab, uiPlayerResultParent);
                uiPlayerResult.BTPlayer = btPlayer;
                gridResult.Add(uiPlayerResult);
            }
        }

        public void InitRankings(string authorName)
        {
            if (gridResult.Count == 0) CreateRankings();
            
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                uiPlayerResult.InitUI();
                if (uiPlayerResult.BTPlayer.Name.Equals(authorName))
                {
                    currentAuthor = uiPlayerResult.BTPlayer;
                    uiPlayerResult.SetAuthor();
                }
            }
        }

        public void ComputeRankings()
        {
            rewindResult = new Dictionary<BTPlayer, RewindData>();
            
            BTSettings settings = GameManager.Instance.GetCurrentSettings();
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                BTPlayer btPlayer = uiPlayerResult.BTPlayer;
                rewindResult[btPlayer] = new RewindData
                {
                    Score = btPlayer.Score,
                    Streak = btPlayer.Streak,
                    ResultType = uiPlayerResult.ResultType,
                    UseFreeze = uiPlayerResult.UseFreeze
                };
                
                if (currentAuthor == uiPlayerResult.BTPlayer) continue;
                
                btPlayer.Score += btPlayer.GetFutureAddScore(uiPlayerResult.ResultType);

                if (uiPlayerResult.ResultType != ResultType.NotFound)
                {
                    btPlayer.Streak++;
                    btPlayer.Streak = Mathf.Min(btPlayer.Streak, settings.StreakMax);
                }
                else if (uiPlayerResult.UseFreeze) btPlayer.StreakFreeze--;
                else btPlayer.Streak = 0;
            }
            
            gridResult.Sort();
            for (int i = 0; i < gridResult.Count; i++)
            {
                gridResult[i].transform.SetSiblingIndex(i);
            }
            
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                uiPlayerResult.FinalUI();
            }
        }

        public void RewindRankings()
        {
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                RewindData data = rewindResult[uiPlayerResult.BTPlayer];
                uiPlayerResult.BTPlayer.Score = data.Score;
                uiPlayerResult.BTPlayer.Streak = data.Streak;
                uiPlayerResult.BTPlayer.StreakFreeze += data.UseFreeze ? 1 : 0;
            }
            
            gridResult.Sort();
            for (int i = 0; i < gridResult.Count; i++)
            {
                gridResult[i].transform.SetSiblingIndex(i);
            }
            
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                uiPlayerResult.InitUI();
                
                RewindData data = rewindResult[uiPlayerResult.BTPlayer];
                uiPlayerResult.RewindUI(data.ResultType, data.UseFreeze);
                
                if (uiPlayerResult.BTPlayer == currentAuthor) uiPlayerResult.SetAuthor();
            }
        }
    }
    
    public class RewindData
    {
        public int Score;
        public int Streak;
        public ResultType ResultType;
        public bool UseFreeze;
    }
}