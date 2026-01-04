using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class TransformScaleYTweenerTrack : TweenerTrack<Transform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateLocalScaleY(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localScale.y;
		}
	}
}
