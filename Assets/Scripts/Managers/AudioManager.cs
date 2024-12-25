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

                Debug.Log($"[AudioManager] Found 3 musics in a row for {ai1.Author}");
                
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
