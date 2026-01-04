using UnityEngine;

namespace Tweening
{
	public sealed class TransformRotationTweenerTrack : TweenerTrack<Transform, Vector3>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.localEulerAngles = Vector3.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localEulerAngles;
		}
	}
}
