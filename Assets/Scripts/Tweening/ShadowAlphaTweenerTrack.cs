using Tweening.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
	public sealed class ShadowAlphaTweenerTrack : TweenerTrack<Shadow, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateColorAlpha(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.effectColor.a;
		}
	}
}
