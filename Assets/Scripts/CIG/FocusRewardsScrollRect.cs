using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class FocusRewardsScrollRect : RewardsScrollRectBase
	{
		[SerializeField]
		private AnimationCurve _itemScaleCurve;

		[SerializeField]
		private AnimationCurve _snapAnimationCurve;

		[SerializeField]
		private AnimationCurve _slideInSnapAnimationCurve;

		private Coroutine _slideInSnapRoutine;

		public void Initialize(Transform itemParent, RewardItem prefab, List<RewardItemData> data)
		{
			List<RewardScrollRectItem> list = new List<RewardScrollRectItem>();
			int i = 0;
			for (int count = data.Count; i < count; i++)
			{
				RewardItem rewardItem = Object.Instantiate(prefab, itemParent);
				rewardItem.Initialize(data[i]);
				list.Add(rewardItem);
			}
			Initialize(list);
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			ScaleAllItems();
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			base.OnBeginDrag(eventData);
			if (_snapRoutine != null)
			{
				StopCoroutine(_snapRoutine);
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			float num = float.MaxValue;
			int num2 = -1;
			int i = 0;
			for (int count = _items.Count; i < count; i++)
			{
				RewardScrollRectItem rewardScrollRectItem = _items[i];
				if (rewardScrollRectItem.gameObject.activeSelf)
				{
					float num3 = Mathf.Abs(rewardScrollRectItem.transform.position.x - base.transform.position.x);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
			}
			if (num2 >= 0)
			{
				_snapRoutine = StartCoroutine(SnapRoutine(GetDeltaPositionToCenter(num2), _snapAnimationCurve, 0.5f));
			}
		}

		protected override void FinishAnimation()
		{
			if (_items.Count > 0)
			{
				int itemIndex = _items.Count / 2;
				if (_slideInSnapRoutine != null)
				{
					StopCoroutine(_slideInSnapRoutine);
				}
				_slideInSnapRoutine = StartCoroutine(SnapRoutine(GetDeltaPositionToCenter(itemIndex), _backToCenterSnapAnimationCurve, 0.5f));
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
				int itemIndex = 0;
				Vector3 v = _itemParent.anchoredPosition;
				v.x = GetDeltaPositionToCenter(itemIndex);
				_itemParent.anchoredPosition = v;
			}
			yield return new WaitForSeconds(_startDelay);
			int i = 0;
			for (int c = _items.Count; i < c; i++)
			{
				RewardScrollRectItem item = _items[i];
				item.AnimateIn();
				if (_slideInSnapRoutine != null)
				{
					StopCoroutine(_slideInSnapRoutine);
				}
				_slideInSnapRoutine = StartCoroutine(SnapRoutine(GetDeltaPositionToCenter(i), _slideInSnapAnimationCurve, 0.5f));
				yield return new WaitWhile(() => item.IsAnimating);
				yield return new WaitForSeconds(_slideItemDelay);
			}
			FinishAnimation();
		}

		private void ScaleAllItems()
		{
			if (_items != null)
			{
				int i = 0;
				for (int count = _items.Count; i < count; i++)
				{
					RewardScrollRectItem rewardScrollRectItem = _items[i];
					float time = Mathf.Abs(rewardScrollRectItem.transform.position.x - base.transform.position.x);
					rewardScrollRectItem.SetScale(_itemScaleCurve.Evaluate(time));
				}
			}
		}
	}
}
