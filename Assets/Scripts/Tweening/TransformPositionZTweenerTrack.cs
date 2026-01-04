using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformPositionZTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionZ(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.z;
		}
	}
}
