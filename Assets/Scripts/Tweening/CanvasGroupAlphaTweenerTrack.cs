using UnityEngine;

namespace Tweening
{
	public sealed class CanvasGroupAlphaTweenerTrack : TweenerTrack<CanvasGroup, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.alpha = Mathf.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.alpha;
		}
	}
}
