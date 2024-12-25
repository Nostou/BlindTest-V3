using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public enum MenuType
    {
        Start,
        Music,
        Result,
        Final
    }
    
    public class UIManager : MonoBehaviour
    {
        public UIMenuStart GetMenuStart() => uiMenuStart;
        public UIMenuMusic GetMenuMusic() => uiMenuMusic;
        public UIMenuResult GetMenuResult() => uiMenuResult;

        public static UIManager Instance { get; private set; }

        private MenuType currentMenu;
        
        [SerializeField] private UIMenuStart uiMenuStart;
        [SerializeField] private UIMenuMusic uiMenuMusic;
        [SerializeField] private UIMenuResult uiMenuResult;
        [SerializeField] private UIMenuFinal uiMenuFinal;
        [SerializeField] private UILoadingScreen uiLoadingScreen;

        private Dictionary<MenuType, GameObject> menus = new Dictionary<MenuType, GameObject>();

        private void Awake()
        {
            Instance = this;
            
            menus.Add(MenuType.Start, uiMenuStart.gameObject);
            menus.Add(MenuType.Music, uiMenuMusic.gameObject);
            menus.Add(MenuType.Result, uiMenuResult.gameObject);
            menus.Add(MenuType.Final, uiMenuFinal.gameObject);
            
            currentMenu = MenuType.Start;
        }

        public void LoadMenu(MenuType menuType)
        {
            uiLoadingScreen.Load(menus[currentMenu], menus[menuType]);
            currentMenu = menuType;
        }
    }
}