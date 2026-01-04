using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformPositionYTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdatePositionY(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.position.y;
		}
	}
}
