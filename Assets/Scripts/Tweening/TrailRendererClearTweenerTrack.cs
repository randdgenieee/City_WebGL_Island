using UnityEngine;

namespace Tweening
{
	public sealed class TrailRendererClearTweenerTrack : TweenerTrack<TrailRenderer, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			if (Mathf.Approximately(evaluatedTime, 1f))
			{
				_component.Clear();
			}
		}

		protected override void ResetDynamicFromValue()
		{
			_component.Clear();
		}
	}
}
