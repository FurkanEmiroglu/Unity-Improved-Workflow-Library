#if IW_DOTWEEN_EXTENSIONS
using DG.Tweening;
using UnityEngine;

namespace IW.DotweenExtensions
{
    public class DoScaleCommand : TweenCommand
    {
        [SerializeField]
        private float _scaleFactor = 1;

        [SerializeField]
        private float _scaleDuration = 0.35f;

        [SerializeField]
        private Ease _ease = DOTween.defaultEaseType;

        [SerializeField]
        private int _loopCount;

        [SerializeField]
        private LoopType _loopType;

        public override void ExecuteCommand(GameObject gameObject)
        {
            Vector3 originalScale = gameObject.transform.localScale;
            Tween t = gameObject.transform.DOScale(originalScale * _scaleFactor, _scaleDuration).SetLink(gameObject).SetEase(_ease);

            if (_loopCount != 0)
                t.SetLoops(_loopCount, _loopType);
            
            if (DOTween.defaultAutoPlay == AutoPlay.None)
                t.Play();
        }
    }
}
#endif