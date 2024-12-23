using TMPro;
using UnityEngine;

namespace UI
{
    public class UIPlayerStart : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtPlayerName;

        public void Init(string playerName, int nbMusic)
        {
            txtPlayerName.text = $"{playerName} ({nbMusic})";
        }
    }
}