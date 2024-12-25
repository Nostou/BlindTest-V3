using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Attributes;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public Action<int, int> OnSongLoaded;
        public Action OnSongStarted;
        public Action<int> OnSongTick;
        public Action OnSongEnded;
        
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField, ReadOnly] private List<AudioInfo> audioInfos = new List<AudioInfo>();

        private int currentSongIndex = -1;
        private AudioInfo currentAudioInfo;

        private Coroutine playSongCoroutine;
        
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
            
            OnSongLoaded?.Invoke(currentSongIndex, audioInfos.Count);
            
            if (audioSource.isPlaying) yield return FadeOut();
            audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
            playSongCoroutine = StartCoroutine(PlaySong());
        }
        
        private IEnumerator PlaySong()
        {
            OnSongStarted?.Invoke();
            StartCoroutine(FadeIn(50));

            int duration = GameManager.Instance.GetCurrentSettings().Time;

            while (duration > 0)
            {
                OnSongTick?.Invoke(duration);
                yield return new WaitForSeconds(1);
                duration--;
            }
            
            yield return StartCoroutine(FadeOut());
            OnSongEnded?.Invoke();
        }

        public void ToResult()
        {
            if (playSongCoroutine != null) StopCoroutine(playSongCoroutine);
            StartCoroutine(FadeIn(20));
        }

        private IEnumerator FadeIn(float target)
        {
            audioSource.volume = 0;
            audioSource.Play();
            Tween fadeTween = audioSource.DOFade(target, fadeDuration);
            yield return fadeTween.WaitForCompletion();
        }

        private IEnumerator FadeOut()
        {
            Tween fadeTween = audioSource.DOFade(0, fadeDuration);
            yield return fadeTween.WaitForCompletion();
            audioSource.Pause();
        }
        
        public void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
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

        private void ShuffleSongs(bool isSmartRandom)
        {
            audioInfos.Shuffle();

            if (!isSmartRandom) return;
            List<AudioInfo> endList = new List<AudioInfo>();
            foreach (BTPlayer player in GameManager.Instance.Players)
            {
                for (int i = 0; i < audioInfos.Count; i++)
                {
                    if (audioInfos[i].Author.Equals(player.Name))
                    {
                        endList.Add(audioInfos[i]);
                        audioInfos.RemoveAt(i);
                        break;
                    }
                }
            }
            
            endList.Shuffle();
            audioInfos.AddRange(endList);
            
            for (int i = 0; i < audioInfos.Count - 2; i++)
            {
                AudioInfo ai1 = audioInfos[i];
                AudioInfo ai2 = audioInfos[i+1];
                AudioInfo ai3 = audioInfos[i+2];

                if (!ai1.Author.Equals(ai2.Author) || !ai2.Author.Equals(ai3.Author)) continue;
                
                for (int j = i + 3; j < audioInfos.Count; j++)
                {
                    if (audioInfos[j].Author.Equals(ai1.Author)) continue;
                    (audioInfos[i+2], audioInfos[j]) = (audioInfos[j], audioInfos[i+2]);
                    break;
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
