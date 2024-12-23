using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIMenuStart : MonoBehaviour
    {
        [SerializeField] private ButtonLongPressListener btnStart;
        [SerializeField] private TMP_Text txtFileLoaded;
        [SerializeField] private TMP_Text txtPath;
        [SerializeField] private TMP_Text txtNbPlayers;
        [SerializeField] private UIPlayerStart playerStartGO;
        [SerializeField] private Transform playerStartParent;

        private void Start()
        {
            btnStart.OnLongPress += OnClickStart;
        }

        private void OnClickStart()
        {
            Debug.Log("Start!!!!");
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
        }

        public void RefreshPlayerList()
        {
            foreach (BTPlayer player in GameManager.Instance.Players)
            {
                UIPlayerStart go = Instantiate(playerStartGO, playerStartParent);
                go.Init(player.Name, player.MusicCount);
            }

            txtNbPlayers.text = $"Players ({GameManager.Instance.Players.Count})";
        }
    }
}
