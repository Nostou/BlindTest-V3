using UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public UIMenuStart GetMenuStart() => uiMenuStart;
        
        public static UIManager Instance { get; private set; }
        
        [SerializeField] private UIMenuStart uiMenuStart;

        private void Awake()
        {
            Instance = this;
        }
    }
}