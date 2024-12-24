using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Attributes;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource audioSource;
        [SerializeField, ReadOnly] private List<AudioInfo> audioInfos = new List<AudioInfo>();

        private AudioClip currentAudioClip;
        private int currentSongIndex = -1;
        private AudioInfo currentAudioInfo;
        
        private void Awake()
        {
            Instance = this;
        }

        public void StartBlindTest()
        {
            ShuffleSongs(GameManager.Instance.GetCurrentSettings().MusicSmartRandom);
            NextSong();
        }

        public void NextSong()
        {
            currentSongIndex++;
            currentAudioInfo = audioInfos[currentSongIndex];
            StartCoroutine(PlaySong());
        }

        private IEnumerator PlaySong()
        {
            AudioType audioType = Path.GetExtension(currentAudioInfo.Path) switch
            {
                ".mp3" => AudioType.MPEG,
                ".ogg" => AudioType.OGGVORBIS,
                _ => AudioType.WAV
            };
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + currentAudioInfo.Path, audioType);

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load audio file: {uwr.error}");
                yield break;
            }
            
            currentAudioClip = DownloadHandlerAudioClip.GetContent(uwr);
            audioSource.clip = currentAudioClip;
            audioSource.Play();
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
                    Title = fileName,
                    Path = af
                });
            }
        
            Debug.Log($"[AudioManager] Found {audioInfos.Count} audio files");;
            return audioInfos;
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
            public string Path;
        }
    }
}
