using UnityEngine;

namespace Tweening
{
	public sealed class MonoBehaviourToggleTweenerTrack : TweenerTrack<MonoBehaviour, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			bool flag = Mathf.Approximately(Mathf.LerpUnclamped(_from ? 1f : 0f, _to ? 1f : 0f, evaluatedTime), 1f);
			if (_component.enabled != flag)
			{
				_component.enabled = flag;
			}
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.enabled;
		}
	}
}
