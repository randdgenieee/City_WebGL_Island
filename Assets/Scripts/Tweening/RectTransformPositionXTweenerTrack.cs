using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class RectTransformPositionXTweenerTrack : TweenerTrack<RectTransform, float, RectTransform>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionX(Mathf.LerpUnclamped(_from, _to.position.x, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.x;
		}
	}
}
