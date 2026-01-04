using Tweening.Extensions;
using UnityEngine;

namespace Tweening
{
	public sealed class SpriteRendererAlphaTweenerTrack : TweenerTrack<SpriteRenderer, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.UpdateColorAlpha(Mathf.LerpUnclamped(_from, _to, evaluatedTime));
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.color.a;
		}
	}
}
