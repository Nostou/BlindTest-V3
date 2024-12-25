using System;
using Managers;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuStart : MonoBehaviour
    {
        [SerializeField] private ButtonLongPressListener btnStart;
        
        [Header("Setup")]
        [SerializeField] private TMP_Text txtFileLoaded;
        [SerializeField] private TMP_Text txtPath;
        [SerializeField] private TMP_Text txtNbPlayers;
        [SerializeField] private UIPlayerStart playerStartGO;
        [SerializeField] private Transform playerStartParent;

        [Header("Settings")]
        [SerializeField] private Slider sliderTime;
        [SerializeField] private TMP_Text txtTime;
        [SerializeField] private TMP_InputField ifScoreGolden;
        [SerializeField] private TMP_InputField ifScoreFirst;
        [SerializeField] private TMP_InputField ifScoreSecond;
        [SerializeField] private Toggle toggleStreakEnabled;
        [SerializeField] private TMP_InputField ifStreakValue;
        [SerializeField] private TMP_InputField ifStreakMax;
        [SerializeField] private TMP_InputField ifStreakFreeze;
        [SerializeField] private Toggle toggleMusicSmartRandom;
        [SerializeField] private Toggle toggleRankingsSecretMode;
        [SerializeField] private ButtonLongPressListener btnReset;

        private void Awake()
        {
            btnStart.OnLongPress += OnClickStart;
            sliderTime.onValueChanged.AddListener(OnSliderTimeChanged);
            ifScoreGolden.onEndEdit.AddListener((s) => OnEndEditScore(ifScoreGolden));
            ifScoreFirst.onEndEdit.AddListener((s) => OnEndEditScore(ifScoreFirst));
            ifScoreSecond.onEndEdit.AddListener((s) => OnEndEditScore(ifScoreSecond));
            ifStreakValue.onEndEdit.AddListener((s) => ClampInput(ifStreakValue, 0, 1000));
            ifStreakMax.onEndEdit.AddListener((s) => ClampInput(ifStreakMax, -1, 1000));
            ifStreakFreeze.onEndEdit.AddListener((s) => ClampInput(ifStreakFreeze, 0 , 1000));
            btnReset.OnLongPress += ResetSettings;
            
            FileBrowser.Instance.OnFolderSelected += ResetPlayerList;
        }

        private int GetValue(TMP_InputField inputField)
        {
            return int.TryParse(inputField.text, out int value) ? value : 0;
        }

        public void SetLoadInfo(string path, int nbFiles)
        {
            txtFileLoaded.text = nbFiles switch
            {
                0 => "No files loaded",
                1 => "Loaded 1 file",
                _ => $"Loaded {nbFiles} files"
            };

            txtPath.gameObject.SetActive(true);
            txtPath.text = path;
            RefreshPlayerList();
        }

        private void RefreshPlayerList()
        {
            foreach (BTPlayer player in GameManager.Instance.Players)
            {
                UIPlayerStart go = Instantiate(playerStartGO, playerStartParent);
                go.Init(player.Name, player.MusicCount);
            }

            txtNbPlayers.text = $"Players ({GameManager.Instance.Players.Count})";
        }

        private void ResetPlayerList()
        {
            while (playerStartParent.childCount > 0)
            {
                DestroyImmediate(playerStartParent.GetChild(0).gameObject);
            }
            
            txtNbPlayers.text = "Players (0)";
        }
        
        private void OnClickStart()
        {
            SaveSettings();
            UIManager.Instance.LoadMenu(MenuType.Song);
            AudioManager.Instance.StartBlindTest();
        }

        public void LockStart(bool state)
        {
            btnStart.SetInteractable(!state);
        }

        private void SaveSettings()
        {
            BTSettingsSO settings = GameManager.Instance.GetCurrentSettings();
            settings.Time = (int)(sliderTime.value * 5) + 10;
            settings.ScoreGolden = GetValue(ifScoreGolden);
            settings.ScoreFirst = GetValue(ifScoreFirst);
            settings.ScoreSecond = GetValue(ifScoreSecond);
            settings.StreakEnabled = toggleStreakEnabled.isOn;
            settings.StreakValue = GetValue(ifStreakValue);
            settings.StreakMax = GetValue(ifStreakMax);
            settings.StreakFreeze = GetValue(ifStreakFreeze);
            settings.MusicSmartRandom = toggleMusicSmartRandom.isOn;
            settings.RankingsSecretMode = toggleRankingsSecretMode.isOn;
        }

        private void ResetSettings()
        {
            //BTSettingsSO settings = Resources.Load<BTSettingsSO>("BT/DefaultSettings");
            BTSettingsSO settings = GameManager.Instance.GetDefaultSettings();
            sliderTime.value = (float)(settings.Time - 10) / 5;
            ifScoreGolden.text = settings.ScoreGolden.ToString();
            ifScoreFirst.text = settings.ScoreFirst.ToString();
            ifScoreSecond.text = settings.ScoreSecond.ToString();
            toggleStreakEnabled.isOn = settings.StreakEnabled;
            ifStreakValue.text = settings.StreakValue.ToString();
            ifStreakMax.text = settings.StreakMax.ToString();
            ifStreakFreeze.text = settings.StreakFreeze.ToString();
            toggleMusicSmartRandom.isOn = settings.MusicSmartRandom;
            toggleRankingsSecretMode.isOn = settings.RankingsSecretMode;
        }
        
        private void OnSliderTimeChanged(float value)
        {
            txtTime.text = $"{value*5+10}";
        }
        
        private void OnEndEditScore(TMP_InputField inputField)
        {
            ClampInput(inputField, 0, 1000);
            if (GetValue(ifScoreGolden) < GetValue(ifScoreFirst)) ifScoreFirst.text = ifScoreGolden.text;
            if (GetValue(ifScoreFirst) < GetValue(ifScoreSecond)) ifScoreSecond.text = ifScoreFirst.text;
        }
        
        private void ClampInput(TMP_InputField inputField, int minValue, int maxValue)
        {
            float value = Math.Clamp(GetValue(inputField), minValue, maxValue);
            inputField.text = value.ToString();
        }
    }
}
