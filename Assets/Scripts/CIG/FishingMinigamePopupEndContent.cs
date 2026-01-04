using CIG.Translation;
using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class FishingMinigamePopupEndContent : MonoBehaviour
	{
		private const int MaxFish = 28;

		private const float ShowFishDelay = 0.06f;

		[SerializeField]
		private Tweener _enterTweener;

		[SerializeField]
		private RectTransform _fishResultParent;

		[SerializeField]
		private FishResultItem _fishResultPrefab;

		[SerializeField]
		private GridLayoutGroup _fishGridLayout;

		[SerializeField]
		private LayoutElement _fishLayoutElement;

		[SerializeField]
		private NumberTweenHelper _caughtText;

		[SerializeField]
		private NumberTweenHelper _progressText;

		[SerializeField]
		private LocalizedText _goalText;

		[SerializeField]
		private LocalizedText _remainingText;

		[SerializeField]
		private GameObject _continueButton;

		private readonly List<FishResultItem> _fishObjects = new List<FishResultItem>();

		private IEnumerator _randomFishAnimationRoutine;

		public void Initialize(int score, long progress, long goal)
		{
			_goalText.LocalizedString = Localization.Integer(goal);
			_progressText.TweenTo(progress, 0f);
			StartCoroutine(AnimationRoutine(score, progress, goal));
		}

		public void Deinitialize()
		{
			if (_randomFishAnimationRoutine != null)
			{
				StopCoroutine(_randomFishAnimationRoutine);
				_randomFishAnimationRoutine = null;
			}
			int i = 0;
			for (int count = _fishObjects.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_fishObjects[i].gameObject);
			}
			_fishObjects.Clear();
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		private IEnumerator AnimationRoutine(int score, long progress, long goal)
		{
			_continueButton.SetActive(value: false);
			_remainingText.gameObject.SetActive(value: false);
			_fishLayoutElement.minHeight = _fishGridLayout.cellSize.y * Mathf.Ceil((float)Mathf.Min(score, 28) / (float)_fishGridLayout.constraintCount);
			_enterTweener.StopAndReset();
			_enterTweener.Play();
			yield return new WaitWhile(() => _enterTweener.IsPlaying);
			int excess = score - 28;
			for (int i = 0; i < score && i < 28; i++)
			{
				FishResultItem fishResultItem = UnityEngine.Object.Instantiate(_fishResultPrefab, _fishResultParent);
				fishResultItem.Initialize();
				_fishObjects.Add(fishResultItem);
				_caughtText.TweenTo(i, i + 1, 0.06f);
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.FishingSplat);
				yield return new WaitForSecondsRealtime(0.06f);
			}
			if (score > 28)
			{
				float num = (float)excess * 0.06f;
				_fishObjects.Last().ShowExcessText(excess, num);
				_caughtText.TweenTo(28m, score, num);
				yield return new WaitForSecondsRealtime(num);
			}
			yield return new WaitForSecondsRealtime(1f);
			_caughtText.TweenTo(score, decimal.Zero, 1f);
			long newProgress = Math.Min(progress + score, goal);
			_progressText.TweenTo(progress, newProgress, 1f);
			yield return new WaitForSecondsRealtime(1f);
			long num2 = goal - newProgress;
			_remainingText.gameObject.SetActive(num2 > 0);
			_remainingText.LocalizedString = Localization.Format(Localization.Key("fishing_progress_remaining"), Localization.Integer(num2));
			_continueButton.SetActive(value: true);
			StartCoroutine(_randomFishAnimationRoutine = RandomFishAnimationRoutine());
		}

		private IEnumerator RandomFishAnimationRoutine()
		{
			while (_fishObjects.Count > 0)
			{
				yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(1f, 3f));
				_fishObjects.PickRandom().PlaySpriteAnimation();
			}
		}
	}
}
