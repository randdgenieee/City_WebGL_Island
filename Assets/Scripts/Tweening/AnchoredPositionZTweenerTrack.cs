using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class AnchoredPositionZTweenerTrack : TweenerTrack<RectTransform, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateAnchoredPositionZ(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.anchoredPosition3D.z;
		}
	}
}
