using UnityEngine;

namespace Tweening
{
	public sealed class TransformScaleTweenerTrack : TweenerTrack<Transform, Vector3>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.localScale = Vector3.LerpUnclamped(_from, _to, evaluatedTime);
		}

		public void SetFromAndTo(Vector3 from, Vector3 to)
		{
			_from = from;
			_to = to;
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.localScale;
		}
	}
}
