using System;
using System.Collections.Generic;
using Attributes;
using ScriptableObjects;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public List<BTPlayer> Players => players;
        public BTSettingsSO GetCurrentSettings() => currentSettings;
        public BTSettingsSO GetDefaultSettings() => defaultSettings;
        
        public static GameManager Instance { get; private set; }

        [SerializeField] private BTSettingsSO currentSettings;
        [SerializeField] private BTSettingsSO defaultSettings;
        [SerializeField, ReadOnly] private List<BTPlayer> players = new List<BTPlayer>();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FileBrowser.Instance.OnFolderSelected += () => players.Clear();
        }    
        
        public void RegisterPlayers(List<AudioManager.AudioInfo> audioInfos)
        {
            BTPlayer currentPlayer = CreatePlayer(audioInfos[0].Author);
            for (int i = 1; i < audioInfos.Count; i++)
            {
                if (currentPlayer.Name.Equals(audioInfos[i].Author))
                {
                    currentPlayer.MusicCount++;
                    continue;
                }
                
                currentPlayer = CreatePlayer(audioInfos[i].Author);
                Debug.Log($"[GameManager] Register {currentPlayer.Name}");
            }
        }

        private BTPlayer CreatePlayer(string playerName)
        {
            BTPlayer player = new BTPlayer(playerName);
            players.Add(player);
            return player;
        }
    }
    
    [Serializable]
    public class BTPlayer
    {
        public string Name;
        public int Score;
        public int MusicCount;
        
        public BTPlayer(string name)
        {
            Name = name;
            Score = 0;
            MusicCount = 1;
        }
    }
}