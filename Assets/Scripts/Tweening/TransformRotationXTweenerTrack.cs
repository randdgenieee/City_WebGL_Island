using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformRotationXTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateLocalEulerAnglesX(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localEulerAngles.x;
		}
	}
}
