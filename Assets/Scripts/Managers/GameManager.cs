using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public List<BTPlayer> Players => players;
        public BTSettings GetCurrentSettings() => currentSettings;
        public BTSettings GetDefaultSettings() => defaultSettings;
        
        public static GameManager Instance { get; private set; }

        [SerializeField] private BTSettings currentSettings;
        [SerializeField] private BTSettings defaultSettings;
        [SerializeField, ReadOnly] private List<BTPlayer> players = new List<BTPlayer>();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FileBrowser.Instance.OnFolderSelected += () => players.Clear();
        }    
        
        public void RegisterPlayers(string[] audioFiles)
        {
            foreach (string af in audioFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(af);
                string playerName = fileName.Split("_")[0];
                
                BTPlayer player = GetPlayer(playerName) ?? CreatePlayer(playerName);
                player.MusicCount++;
            }
        }

        public BTPlayer GetPlayer(string playerName) => players.FirstOrDefault(p => p.Name.Equals(playerName));
        private BTPlayer CreatePlayer(string playerName)
        {
            BTPlayer player = new BTPlayer(playerName);
            players.Add(player);
            
            Debug.Log($"[GameManager] Create {playerName}");
            return player;
        }
    }
    
    [Serializable]
    public class BTPlayer
    {
        public string Name;
        public int Score;
        public int Streak;
        public int StreakFreeze;
        public int MusicCount;
        
        public BTPlayer(string name)
        {
            Name = name;
            Score = 0;
            Streak = 0;
            StreakFreeze = GameManager.Instance.GetCurrentSettings().StreakFreeze;
            MusicCount = 0;
        }

        public int GetFutureAddScore(ResultType resultType)
        {
            BTSettings settings = GameManager.Instance.GetCurrentSettings();
            float addScore = 0;
            if (resultType == ResultType.Golden) addScore = settings.ScoreGolden;
            else if (resultType == ResultType.First) addScore = settings.ScoreFirst;
            else if (resultType == ResultType.Second) addScore = settings.ScoreSecond;

            if (settings.StreakEnabled) addScore *= 1 + Streak * settings.StreakValue / 100.0f;
            return (int)addScore;
        }
    }
}