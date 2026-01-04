using UnityEngine;

namespace Tweening
{
	public sealed class AnchoredPositionTweenerTrack : TweenerTrack<RectTransform, Vector2>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.anchoredPosition = Vector2.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.anchoredPosition;
		}
	}
}
