using System;
using System.Collections.Generic;
using System.IO;
using Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField, ReadOnly] private List<AudioInfo> audioInfos = new List<AudioInfo>();

        private void Awake()
        {
            Instance = this;
        }

        public List<AudioInfo> CreateAudioInfos(string[] audioFiles)
        {
            audioInfos.Clear();
            
            foreach (string af in audioFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(af);
                audioInfos.Add(new AudioInfo
                {
                    Author = fileName.Split("_")[0],
                    Title = fileName
                });
            }
        
            Debug.Log($"[AudioManager] Found {audioInfos.Count} audio files");;
            return audioInfos;
        }

        public void StartBlindTest()
        {
            ShuffleSongs(false);
        }

        private void ShuffleSongs(bool isTrueRandom)
        {
            //True shuffle
            for (int i = 0; i < audioInfos.Count; i++)
            {
                int rdm = Random.Range(0, audioInfos.Count);
                (audioInfos[rdm], audioInfos[i]) = (audioInfos[i], audioInfos[rdm]);
            }

            if (isTrueRandom) return;
            //Make sure each player has one song in the end
            List<BTPlayer> players = GameManager.Instance.Players;
            foreach (BTPlayer player in players)
            {
                for (int i = audioInfos.Count-1; i >= 0; i--)
                {
                    AudioInfo ai = audioInfos[i];
                    if (player.Name.Equals(ai.Author))
                    {
                        audioInfos.Remove(ai);
                        audioInfos.Add(ai);
                        break;
                    }
                }
            }
            
            //Avoid 3 songs in a row from the same player
            for (int i = 0; i < audioInfos.Count-2; i++)
            {
                AudioInfo ai1 = audioInfos[i];
                AudioInfo ai2 = audioInfos[i+1];
                AudioInfo ai3 = audioInfos[i+2];
                if (ai1.Author.Equals(ai2.Author) && ai2.Author.Equals(ai3.Author))
                {
                    int rdm = Random.Range(0, audioInfos.Count-players.Count);
                    (audioInfos[i+1], audioInfos[i+2]) = (audioInfos[i+2], audioInfos[i+1]);
                }
            }
        }
    
        [Serializable]
        public class AudioInfo
        {
            public string Author;
            public string Title;
        }
    }
}
