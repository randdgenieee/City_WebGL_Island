using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformRotationZTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateLocalEulerAnglesZ(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localEulerAngles.z;
		}
	}
}
