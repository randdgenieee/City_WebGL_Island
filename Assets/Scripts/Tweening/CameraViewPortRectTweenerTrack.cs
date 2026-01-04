using UnityEngine;

namespace Tweening
{
	public class CameraViewPortRectTweenerTrack : TweenerTrack<Camera, Rect>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			_component.rect = LerpRectUnclamped(_from, _to, evaluatedTime);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.rect;
		}

		private Rect LerpRectUnclamped(Rect from, Rect to, float t)
		{
			return new Rect(Vector2.LerpUnclamped(from.position, to.position, t), Vector2.LerpUnclamped(from.size, to.size, t));
		}
	}
}
