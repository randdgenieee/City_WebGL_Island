using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
	public sealed class ShadowColorTweenerTrack : TweenerTrack<Shadow, Color>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.effectColor = Color.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.effectColor;
		}
	}
}
