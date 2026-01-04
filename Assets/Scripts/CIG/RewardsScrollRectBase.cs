using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class RewardsScrollRectBase : ScrollRect
	{
		public delegate void AnimationFinishedEventHandler();

		[SerializeField]
		protected RectTransform _itemParent;

		[SerializeField]
		protected AnimationCurve _backToCenterSnapAnimationCurve;

		[SerializeField]
		protected float _startDelay;

		[SerializeField]
		protected float _slideItemDelay = 0.2f;

		[SerializeField]
		protected GameObject _animationOverlay;

		protected readonly List<RewardScrollRectItem> _items = new List<RewardScrollRectItem>();

		protected Coroutine _snapRoutine;

		private Coroutine _animationRoutine;

		public event AnimationFinishedEventHandler AnimationFinishedEvent;

		private void FireAnimationFinishedEvent()
		{
			if (this.AnimationFinishedEvent != null)
			{
				this.AnimationFinishedEvent();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_animationOverlay.SetActive(value: false);
		}

		public void Initialize(List<RewardScrollRectItem> items)
		{
			_items.AddRange(items);
			int i = 0;
			for (int count = _items.Count; i < count; i++)
			{
				_items[i].ResetAnimation(resetToEnd: false);
			}
		}

		public virtual void Deinitialize()
		{
			if (_items != null)
			{
				int i = 0;
				for (int count = _items.Count; i < count; i++)
				{
					UnityEngine.Object.Destroy(_items[i].gameObject);
				}
				_items.Clear();
			}
		}

		public void StartAnimation()
		{
			if (_animationRoutine != null)
			{
				StopCoroutine(_animationRoutine);
			}
			_animationRoutine = StartCoroutine(AnimationRoutine());
		}

		public void OnAnimationOverlayClicked()
		{
			FinishAnimation();
		}

		protected virtual void FinishAnimation()
		{
			_animationOverlay.SetActive(value: false);
			if (_animationRoutine != null)
			{
				StopCoroutine(_animationRoutine);
				_animationRoutine = null;
			}
			int i = 0;
			for (int count = _items.Count; i < count; i++)
			{
				_items[i].ResetAnimation(resetToEnd: true);
			}
			base.horizontal = true;
			FireAnimationFinishedEvent();
		}

		protected float GetDeltaPositionToCenter(int itemIndex)
		{
			float width = _itemParent.rect.width;
			float x = ((RectTransform)_items[itemIndex].transform).anchoredPosition.x;
			return width / 2f - x;
		}

		protected IEnumerator SnapRoutine(float deltaPosition, AnimationCurve animationCurve, float duration)
		{
			float curveDuration = animationCurve.keys[animationCurve.length - 1].time;
			float initialPosition = _itemParent.anchoredPosition.x;
			float timeScale = 1f / (duration / curveDuration);
			float time = 0f;
			while (time < curveDuration)
			{
				time += Time.deltaTime * timeScale;
				Vector3 v = _itemParent.anchoredPosition;
				v.x = Mathf.Lerp(initialPosition, deltaPosition, animationCurve.Evaluate(time));
				_itemParent.anchoredPosition = v;
				yield return null;
			}
		}

		protected abstract IEnumerator AnimationRoutine();
	}
}
