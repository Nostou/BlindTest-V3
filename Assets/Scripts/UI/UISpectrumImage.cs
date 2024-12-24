using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISpectrumImage : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            fillImage.fillAmount = 0f;
        }

        public void SetDimensions(float width, float height)
        {
            fillImage.rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void Fill(float amount, float duration = 0.0f)
        {
            if (duration == 0) fillImage.fillAmount = Mathf.Clamp(amount, 0f, 1f);
            else
            {
                fillImage.DOKill();
                fillImage.DOFillAmount(amount, duration);
            }
        }
    }
}