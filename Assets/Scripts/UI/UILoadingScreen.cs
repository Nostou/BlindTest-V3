using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image imgLoading;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float transitionDuration = 0.1f;

        public void Load(GameObject goToDisable, GameObject goToEnable)
        {
            imgLoading.color = new Color(imgLoading.color.r, imgLoading.color.g, imgLoading.color.b, 0);
            imgLoading.gameObject.SetActive(true);
            
            Sequence loadSequence = DOTween.Sequence()
                .Append(imgLoading.DOFade(1, fadeDuration))
                .AppendCallback(() =>
                {
                    goToDisable.SetActive(false);
                    goToEnable.SetActive(true);
                })
                .AppendInterval(transitionDuration)
                .Append(imgLoading.DOFade(0, fadeDuration))
                .AppendCallback(() => imgLoading.gameObject.SetActive(false));
        }
    }
}