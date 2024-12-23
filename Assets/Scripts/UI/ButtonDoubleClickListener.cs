using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent (typeof(Button))]
    public class ButtonDoubleClickListener : MonoBehaviour, IPointerClickHandler {

        public Action OnDoubleClick;
        
        [SerializeField, Min(0.01f)] private float doubleClickDuration = 0.3f ;

        private int nbClick;
        private float firstClickTime;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            nbClick++;
            if (nbClick == 1)
            {
                firstClickTime = Time.time;
                return;
            }
            
            float elapsedTime = Time.time - firstClickTime;
            if (elapsedTime > doubleClickDuration)
            {
                nbClick = 1;
                firstClickTime = Time.time;
                return;
            }
            
            if (button.interactable) OnDoubleClick?.Invoke();
            nbClick = 0;
        }

    }
}
