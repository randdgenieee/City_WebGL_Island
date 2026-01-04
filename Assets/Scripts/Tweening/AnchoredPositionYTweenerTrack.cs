using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class AnchoredPositionYTweenerTrack : TweenerTrack<RectTransform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateAnchoredPositionY(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.anchoredPosition.y;
		}
	}
}
