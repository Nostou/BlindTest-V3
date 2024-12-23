using System.Collections.Generic;
using System.IO;
using System.Linq;
using Managers;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class FileBrowser : MonoBehaviour
{
    [SerializeField] private Button btnSelect;

    private void Awake()
    {
        btnSelect.onClick.AddListener(OpenFileBrowser);
    }
    
    private void OpenFileBrowser()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select audio folder", "", false);
        
        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log("[FileBrowser] No folder selected.");
            return;
        }
        
        string selectedPath = paths[0];
        Debug.Log($"[FileBrowser] Folder selected: {selectedPath}");
        
        string[] audioFiles = Directory.GetFiles(selectedPath, "*.*", SearchOption.AllDirectories)
            .Where(file => file.EndsWith(".mp3") || file.EndsWith(".ogg") || file.EndsWith(".wav"))
            .ToArray();
        
        if (audioFiles.Length == 0)
        {
            Debug.Log("[FileBrowser] No audio files found.");
            return;
        }
        
        UIManager.Instance.GetMenuStart().SetLoadInfo(selectedPath, audioFiles.Length);
        List<AudioManager.AudioInfo> audioInfos = AudioManager.Instance.CreateAudioInfos(audioFiles);
        GameManager.Instance.RegisterPlayers(audioInfos);
        UIManager.Instance.GetMenuStart().RefreshPlayerList();
    }
}
