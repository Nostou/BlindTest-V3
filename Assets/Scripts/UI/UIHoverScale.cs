using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float targetScale;
        [SerializeField] private float duration;

        private bool isHoverable = true;
    
        private void TweenText(Vector3 tweenScale)
        {
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(tweenScale, duration);
        }

        public void SetHoverable(bool state)
        {
            isHoverable = state;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isHoverable) return;
            
            TweenText(Vector3.one * targetScale);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isHoverable) return;
            
            TweenText(Vector3.one);
        }
    }
}
