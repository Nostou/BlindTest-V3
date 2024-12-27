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
            BTSettingsSO settings = GameManager.Instance.GetCurrentSettings();
            foreach (UIPlayerResult uiPlayerResult in gridResult)
            {
                BTPlayer btPlayer = uiPlayerResult.BTPlayer;
                btPlayer.Score += btPlayer.GetFutureAddScore(uiPlayerResult.ResultType);

                if (uiPlayerResult.ResultType != ResultType.NotFound)
                {
                    btPlayer.Streak++;
                    btPlayer.Streak = Mathf.Min(btPlayer.Streak, settings.StreakMax);
                }
                
                else if (btPlayer != currentAuthor && !uiPlayerResult.UseFreeze) btPlayer.Streak = 0;
                if (uiPlayerResult.UseFreeze) btPlayer.StreakFreeze--;
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
    }
}