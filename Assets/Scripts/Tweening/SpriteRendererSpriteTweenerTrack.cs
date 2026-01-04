using UnityEngine;

namespace Tweening
{
	public sealed class SpriteRendererSpriteTweenerTrack : TweenerTrack<SpriteRenderer, Sprite>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			if (Mathf.Approximately(Mathf.LerpUnclamped(0f, 1f, evaluatedTime), 1f))
			{
				_component.sprite = _to;
			}
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.sprite;
		}
	}
}
