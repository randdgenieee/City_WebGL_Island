using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class RectTransformPositionYTweenerTrack : TweenerTrack<RectTransform, float, RectTransform>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionY(Mathf.LerpUnclamped(_from, _to.position.y, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.y;
		}
	}
}
