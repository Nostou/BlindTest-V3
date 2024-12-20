using System.IO;
using System.Linq;
using Managers;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class FileBrowser : MonoBehaviour
{
    [SerializeField] private Button btnOpen;

    private void Awake()
    {
        btnOpen.onClick.AddListener(OpenFileBrowser);
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
        AudioManager.Instance.SetAudioInfos(audioFiles);
    }
}
