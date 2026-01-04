using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
	public sealed class ImageColorTweenerTrack : TweenerTrack<Image, Color>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.color = Color.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.color;
		}
	}
}
