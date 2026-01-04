using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformRotationYTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateLocalEulerAnglesY(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localEulerAngles.y;
		}
	}
}
