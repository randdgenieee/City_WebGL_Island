using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
	public sealed class ImageFillTweenerTrack : TweenerTrack<Image, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.fillAmount = Mathf.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.fillAmount;
		}
	}
}
