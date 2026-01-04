using UnityEngine;

namespace Tweening
{
	public sealed class CanvasGroupInteractableTweenerTrack : TweenerTrack<CanvasGroup, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			bool flag = Mathf.Approximately(Mathf.LerpUnclamped(_from ? 1f : 0f, _to ? 1f : 0f, evaluatedTime), 1f);
			if (_component.interactable != flag)
			{
				_component.interactable = flag;
			}
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.interactable;
		}
	}
}
