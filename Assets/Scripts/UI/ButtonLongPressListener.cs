using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(Button))]
    public class ButtonLongPressListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Action OnLongPress;
        
        [SerializeField, Min(0.01f)] public float holdDuration = 0.5f;
        [SerializeField] private Image fillImage;

        private bool isPointerDown;
        private bool isLongPressed;

        private Button button;

        private void OnEnable()
        {
            fillImage.fillAmount = 0;
        }

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            StartCoroutine(PressCoroutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            isLongPressed = false;
            if (fillImage) fillImage.fillAmount = 0;
        }

        public void SetInteractable(bool state)
        {
            button.interactable = state;
        }

        private IEnumerator PressCoroutine()
        {
            float timer = 0;
            while (isPointerDown && !isLongPressed)
            {
                if (!button.interactable) yield break;
                
                timer += Time.deltaTime;
                if (timer >= holdDuration) {
                    isLongPressed = true;
                    OnLongPress?.Invoke();
                    break;
                }
                
                if (fillImage) fillImage.fillAmount = timer / holdDuration;
                yield return null;
            }
        }
    }
}
