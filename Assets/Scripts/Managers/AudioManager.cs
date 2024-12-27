using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Attributes;
using DG.Tweening;
using Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public Action OnMusicStarted;
        public Action<int> OnMusicTick;
        public Action OnMusicEnded;
        
        public static AudioManager Instance { get; private set; }
        
        public float Volume => audioSource.volume;
        public int MusicIndex => currentMusicIndex;
        public int MusicCount => audioInfos.Count;
        public AudioInfo CurrentMusic => currentAudioInfo;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float waitBuffer = 0.5f;
        [SerializeField, ReadOnly] private List<AudioInfo> audioInfos = new List<AudioInfo>();

        private int currentMusicIndex = -1;
        private AudioInfo currentAudioInfo;

        private Coroutine playMusicCoroutine;
        private bool isPlaying;
        
        private void Awake()
        {
            Instance = this;
        }

        public void StartBlindTest()
        {
            ShuffleMusics(GameManager.Instance.GetCurrentSettings().MusicSmartRandom);
            NextMusic();
        }

        public void NextMusic()
        {
            currentMusicIndex++;
            currentAudioInfo = audioInfos[currentMusicIndex];
            StartCoroutine(PrepareMusic());
        }

        private IEnumerator PrepareMusic()
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
            
            if (audioSource.isPlaying) yield return FadeOut(0.0f);
            yield return new WaitForSeconds(waitBuffer);
            
            if (audioSource.clip) Destroy(audioSource.clip);
            
            audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
            playMusicCoroutine = StartCoroutine(PlayMusic());
        }
        
        private IEnumerator PlayMusic()
        {
            isPlaying = true;
            OnMusicStarted?.Invoke();
            StartCoroutine(FadeIn(0.5f));

            int duration = GameManager.Instance.GetCurrentSettings().Time;

            while (duration > 0)
            {
                OnMusicTick?.Invoke(duration);
                yield return new WaitForSeconds(1);
                duration--;
            }
            
            OnMusicTick?.Invoke(0);
            yield return StartCoroutine(FadeOut(0.0f));
            OnMusicEnded?.Invoke();
            isPlaying = false;
        }

        public void ToResult()
        {
            if (isPlaying)
            {
                StopCoroutine(playMusicCoroutine);
                OnMusicEnded?.Invoke();
                isPlaying = false;
                StartCoroutine(FadeOut(0.15f, false));
            }
            else StartCoroutine(FadeIn(0.15f));
        }

        private IEnumerator FadeIn(float target)
        {
            audioSource.volume = 0;
            audioSource.Play();
            audioSource.DOKill();
            Tween fadeTween = audioSource.DOFade(target, fadeDuration);
            yield return fadeTween.WaitForCompletion();
        }

        private IEnumerator FadeOut(float target, bool pauseMusic = true)
        {
            audioSource.DOKill();
            Tween fadeTween = audioSource.DOFade(target, fadeDuration);
            yield return fadeTween.WaitForCompletion();
            if (pauseMusic) audioSource.Pause();
        }
        
        public void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume/100, 0, 1);
            audioSource.volume = volume;
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

        private void ShuffleMusics(bool isSmartRandom)
        {
            if (!isSmartRandom)
            {
                audioInfos.Shuffle();
                return;
            }

            List<BTPlayer> players = GameManager.Instance.Players;
            int nbMusicPerPlayer = audioInfos.Count / players.Count;
            
            Dictionary<BTPlayer, List<AudioInfo>> firstHalf = new Dictionary<BTPlayer, List<AudioInfo>>();
            Dictionary<BTPlayer, List<AudioInfo>> secondHalf = new Dictionary<BTPlayer, List<AudioInfo>>();
            
            foreach (BTPlayer player in players)
            {
                firstHalf.Add(player, new List<AudioInfo>());
                secondHalf.Add(player, new List<AudioInfo>());
            }

            foreach (AudioInfo audioInfo in audioInfos)
            {
                BTPlayer player = players.Find(p => p.Name.Equals(audioInfo.Author));
                if (firstHalf[player].Count < (nbMusicPerPlayer-1)/2) firstHalf[player].Add(audioInfo);
                else secondHalf[player].Add(audioInfo);
            }
            
            List<AudioInfo> firstList = new List<AudioInfo>();
            List<AudioInfo> secondList = new List<AudioInfo>();
            
            foreach (BTPlayer player in players)
            {
                firstList.AddRange(firstHalf[player]);
                secondList.AddRange(secondHalf[player]);
            }
            
            firstList.Shuffle();
            secondList.Shuffle();
            
            List<AudioInfo> endList = new List<AudioInfo>();
            foreach (BTPlayer player in players)
            {
                for (int i = 0; i < secondList.Count; i++)
                {
                    if (secondList[i].Author.Equals(player.Name))
                    {
                        endList.Add(secondList[i]);
                        secondList.RemoveAt(i);
                        break;
                    }
                }
            }
            endList.Shuffle();
            secondList.AddRange(endList);
            
            audioInfos.Clear();
            audioInfos.AddRange(firstList);
            audioInfos.AddRange(secondList);
            PreventOccurrencesInARow(audioInfos, 2);
        }

        private void PreventOccurrencesInARow(List<AudioInfo> audioList, int maxOccurrences)
        {
            int consecutiveCount = 1;
            for (int i = 1; i < audioList.Count; i++) 
            {
                if (audioList[i].Author.Equals(audioList[i - 1].Author)) consecutiveCount++;
                else consecutiveCount = 1;

                if (consecutiveCount <= maxOccurrences) continue;
                
                Debug.Log($"[AudioManager] Found more than {maxOccurrences} musics in a row for {audioList[i].Author}");

                for (int j = i + 1; j < audioList.Count; j++)
                {
                    if (!audioList[j].Author.Equals(audioList[i].Author))
                    {
                        (audioList[i], audioList[j]) = (audioList[j], audioList[i]);
                        consecutiveCount = 1;
                        break;
                    }
                }
            }
        }
    
        [Serializable]
        public class AudioInfo
        {
            public string Author; //TODO: Change to BTPlayer
            public string Title;
            public string Path;
        }
    }
}
