using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIPlayerStart : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtPlayerName;

        public void Init(BTPlayer btPlayer)
        {
            txtPlayerName.text = $"{btPlayer.Name} ({btPlayer.MusicCount})";
        }
    }
}