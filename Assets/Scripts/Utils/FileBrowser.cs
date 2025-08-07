using System;
using System.IO;
using System.Linq;
using Managers;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class FileBrowser : MonoBehaviour
    {
        public Action OnFolderSelected;
    
        public static FileBrowser Instance { get; private set; }
    
        [SerializeField] private Button btnSelect;
        [SerializeField] private TMP_Text txtFileLoaded;
        [SerializeField] private TMP_Text txtPath;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            btnSelect.onClick.AddListener(OpenFileBrowser);
        }
    
        private void OpenFileBrowser()
        {
            OnFolderSelected?.Invoke();
        
            string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select audio folder", "", false);
        
            if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
            {
                Debug.Log("[FileBrowser] No folder selected.");
                return;
            }
        
            string selectedPath = paths[0];
            Debug.Log($"[FileBrowser] Folder selected: {selectedPath}");
        
            string[] audioFiles = Directory.GetFiles(selectedPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => file.EndsWith(".mp3") || file.EndsWith(".ogg") || file.EndsWith(".wav"))
                .ToArray();
        
            SetLoadInfo(selectedPath, audioFiles.Length);
        
            if (audioFiles.Length == 0)
            {
                Debug.Log("[FileBrowser] No audio files found.");
                return;
            }
        
            GameManager.Instance.RegisterPlayers(audioFiles);
            AudioManager.Instance.CreateBT(audioFiles);
            UIManager.Instance.GetMenuStart().RefreshPlayerList();
        }
    
        private void SetLoadInfo(string path, int nbFiles)
        {
            txtFileLoaded.text = nbFiles switch
            {
                0 => "No files loaded",
                1 => "Loaded 1 file",
                _ => $"Loaded {nbFiles} files"
            };

            txtPath.text = path;
        }
    }
}
