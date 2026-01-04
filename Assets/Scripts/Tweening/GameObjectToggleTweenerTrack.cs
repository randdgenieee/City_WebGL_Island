using UnityEngine;

namespace Tweening
{
	public sealed class GameObjectToggleTweenerTrack : TweenerTrack<Component, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			bool flag = Mathf.Approximately(Mathf.LerpUnclamped(_from ? 1f : 0f, _to ? 1f : 0f, evaluatedTime), 1f);
			if (_component.gameObject.activeSelf != flag)
			{
				_component.gameObject.SetActive(flag);
			}
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.gameObject.activeSelf;
		}
	}
}
