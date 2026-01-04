using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformLocalPositionXTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateLocalPositionX(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localPosition.x;
		}
	}
}
