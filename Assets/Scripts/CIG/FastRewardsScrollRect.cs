using SparkLinq;
using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class FastRewardsScrollRect : RewardsScrollRectBase
	{
		private const float LastItemFocusOffset = 75f;

		[SerializeField]
		private RectTransform _maskTransform;

		[SerializeField]
		private Tweener _scrollbarTweener;

		public override void Deinitialize()
		{
			base.Deinitialize();
			_scrollbarTweener.StopAndReset();
		}

		protected override void FinishAnimation()
		{
			_scrollbarTweener.Play();
			if (_items.Count > 0)
			{
				if (_snapRoutine != null)
				{
					StopCoroutine(_snapRoutine);
				}
				_snapRoutine = StartCoroutine(SnapRoutine(0f, _backToCenterSnapAnimationCurve, 0.5f));
			}
			base.FinishAnimation();
		}

		protected override IEnumerator AnimationRoutine()
		{
			_animationOverlay.SetActive(value: true);
			base.horizontal = false;
			if (_items.Count > 0)
			{
				yield return null;
				Vector3 v = _itemParent.anchoredPosition;
				float a = GetDeltaPositionToCenter(_items.Count - 1) + _maskTransform.sizeDelta.x / 2f - 75f;
				v.x = Mathf.Min(a, 0f);
				_itemParent.anchoredPosition = v;
			}
			yield return new WaitForSeconds(_startDelay);
			int j = 0;
			for (int c = _items.Count; j < c; j++)
			{
				_items[j].AnimateIn();
				yield return new WaitForSeconds(_slideItemDelay);
			}
			yield return new WaitWhile(() => _items.Any((RewardScrollRectItem i) => i.IsAnimating));
			FinishAnimation();
		}
	}
}
