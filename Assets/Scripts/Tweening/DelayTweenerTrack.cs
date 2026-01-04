using UnityEngine;

namespace Tweening
{
	public sealed class DelayTweenerTrack : TweenerTrack<Component, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
		}

		protected override void ResetDynamicFromValue()
		{
		}
	}
}
