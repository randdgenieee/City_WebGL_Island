using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class RectTransformPositionZTweenerTrack : TweenerTrack<RectTransform, float, RectTransform>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionZ(Mathf.LerpUnclamped(_from, _to.position.z, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.z;
		}
	}
}
