using CIG.Translation;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
    public class SplashSparkling : Loader
    {
        private const string SkipTrigger = "Skip";

        [SerializeField]
        private AudioSource _clip;

        [SerializeField]
        private Animator _splashAnimator;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _fadeDuration = 0.7f;

        [SerializeField]
        private Slider _progressbar;

        private bool _hasLoaded;

        public void ConsentRequestCallback(string message)
        {
            if (message == "cancelled")
            {
                CIGApp.Quit();
            }
            else
            {
                FinishedPlaying();
            }
        }

        protected override void OnStartLoading()
        {
            base.OnStartLoading();
            _clip.Play();
        }

        protected override IEnumerator DoIntroAnimation()
        {
            while (!_hasLoaded)
            {
                yield return null;
            }
        }

        protected override IEnumerator DoOutroAnimation()
        {
            float time = 0f;
            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = 1f - time / _fadeDuration;
                yield return null;
            }
        }

        protected override void SetProgress(float normalisedProgress)
        {
            base.SetProgress(normalisedProgress);
            _progressbar.normalizedValue = normalisedProgress;
        }

        private void SkipAnimation()
        {
            _splashAnimator.SetTrigger("Skip");
            FinishedPlaying();
        }

        private void FinishedPlaying()
        {
            {
                _hasLoaded = true;
                return;
            }
        }
    }
}
