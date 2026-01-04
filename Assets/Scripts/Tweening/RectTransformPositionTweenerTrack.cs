using UnityEngine;

namespace Tweening
{
	public sealed class RectTransformPositionTweenerTrack : TweenerTrack<RectTransform, Vector3, RectTransform>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.position = Vector3.LerpUnclamped(_from, _to.position, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position;
		}
	}
}
