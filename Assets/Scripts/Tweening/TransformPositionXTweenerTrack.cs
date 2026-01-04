using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformPositionXTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionX(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.x;
		}
	}
}
