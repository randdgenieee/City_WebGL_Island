using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TOCILoader : Loader
	{
		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private float _fadeDuration = 0.5f;

		[SerializeField]
		private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private Slider _loadingBar;

		protected override IEnumerator DoIntroAnimation()
		{
			float time = 0f;
			while (time < _fadeDuration)
			{
				time += Time.deltaTime;
				_canvasGroup.alpha = _fadeCurve.Evaluate(time / _fadeDuration);
				yield return null;
			}
		}

		protected override IEnumerator DoOutroAnimation()
		{
			float time = 0f;
			while (time < _fadeDuration)
			{
				time += Time.deltaTime;
				_canvasGroup.alpha = 1f - _fadeCurve.Evaluate(time / _fadeDuration);
				yield return null;
			}
		}

		protected override void SetProgress(float normalisedProgress)
		{
			base.SetProgress(normalisedProgress);
			_loadingBar.value = normalisedProgress;
		}
	}
}
