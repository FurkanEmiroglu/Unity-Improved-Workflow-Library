#if IW_DOTWEEN_EXTENSIONS
using DG.Tweening;
using UnityEngine;

namespace IW.DotweenExtensions
{
    public class ScaleTweenCommand : TweenCommand
    {
        [SerializeField]
        private float _scaleFactor;

        [SerializeField]
        private float _scaleDuration;

        [SerializeField]
        private Ease _ease = DOTween.defaultEaseType;

        public override void ExecuteCommand(GameObject gameObject)
        {
            Vector3 originalScale = gameObject.transform.localScale;
            Tween t = gameObject.transform.DOScale(originalScale * _scaleFactor, _scaleDuration).SetLink(gameObject).SetEase(_ease);

            if (DOTween.defaultAutoPlay == AutoPlay.None)
                t.Play();
        }
    }
}
#endif