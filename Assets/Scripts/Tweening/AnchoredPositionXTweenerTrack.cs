using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class AnchoredPositionXTweenerTrack : TweenerTrack<RectTransform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateAnchoredPositionX(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.anchoredPosition.x;
		}
	}
}
