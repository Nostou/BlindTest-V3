using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

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
        public int MusicCount => musicList.Count;
        public Music CurrentMusic => currentMusic;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float waitBuffer = 0.5f;
        [SerializeField, ReadOnly] private List<Music> musicList = new List<Music>();

        private AudioShuffler shuffler = new AudioShuffler();
        
        private int currentMusicIndex = -1;
        private Music currentMusic;

        private Coroutine playMusicCoroutine;
        private bool isPlaying;
        
        private void Awake()
        {
            Instance = this;
        }

        public void StartBlindTest()
        {
            shuffler.CustomShuffle(musicList);
            NextMusic();
        }

        public void NextMusic()
        {
            currentMusicIndex++;
            currentMusic = musicList[currentMusicIndex];
            StartCoroutine(PrepareMusic());
        }

        private IEnumerator PrepareMusic()
        {
            AudioType audioType = Path.GetExtension(currentMusic.Path) switch
            {
                ".mp3" => AudioType.MPEG,
                ".ogg" => AudioType.OGGVORBIS,
                _ => AudioType.WAV
            };
            
            UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + currentMusic.Path, audioType);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AudioManager] Failed to load audio file: {uwr.error}");
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
        
        public void CreateBT(string[] audioFiles)
        {
            musicList.Clear();

            foreach (string af in audioFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(af);
                string playerName = fileName.Split("_")[0];
                musicList.Add(new Music()
                {
                    Player = GameManager.Instance.GetPlayer(playerName),
                    Title = fileName,
                    Path = af
                });
            }
        
            Debug.Log($"[AudioManager] Found {musicList.Count} audio files");
        }
    }

    [Serializable]
    public class Music
    {
        [InlineProperty] public BTPlayer Player;
        public string Title;
        public string Path;
        
        public string Author => Player.Name;
    }
}
