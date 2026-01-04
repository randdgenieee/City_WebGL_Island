using UnityEngine;

namespace Tweening
{
	public class CameraOrthographicSizeTweenerTrack : TweenerTrack<Camera, float>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.orthographicSize = Mathf.LerpUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.orthographicSize;
		}
	}
}
