using System;
using System.Collections.Generic;
using Attributes;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public List<BTPlayer> Players => players;
        
        public static GameManager Instance { get; private set; }

        [SerializeField, ReadOnly] private List<BTPlayer> players = new List<BTPlayer>();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterPlayers(List<AudioManager.AudioInfo> audioInfos)
        {
            players.Clear();
            
            BTPlayer currentPlayer = null;
            foreach (AudioManager.AudioInfo ai in audioInfos)
            {
                if (currentPlayer != null && currentPlayer.Name.Equals(ai.Author)) continue;
                currentPlayer = new BTPlayer(ai.Author);
                players.Add(currentPlayer);
                Debug.Log($"[GameManager] Register {currentPlayer.Name}");
            }
        }
    }
    
    [Serializable]
    public class BTPlayer
    {
        public string Name;
        public int Score;
        
        public BTPlayer(string name)
        {
            Name = name;
            Score = 0;
        }
    }
}