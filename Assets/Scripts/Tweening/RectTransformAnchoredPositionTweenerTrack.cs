using UnityEngine;

namespace Tweening
{
	public sealed class RectTransformAnchoredPositionTweenerTrack : TweenerTrack<RectTransform, Vector3, Vector3>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.anchoredPosition = Vector3.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.anchoredPosition;
		}
	}
}
