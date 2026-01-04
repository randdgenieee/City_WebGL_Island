using CIG;
using UnityEngine;

namespace Tweening
{
	public sealed class AnimatedSpriteFrameRateTweenerTrack : TweenerTrack<AnimatedSpriteBase, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.FPS = Mathf.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.FPS;
		}
	}
}
