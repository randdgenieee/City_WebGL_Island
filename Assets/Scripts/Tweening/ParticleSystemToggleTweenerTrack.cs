using UnityEngine;

namespace Tweening
{
	public sealed class ParticleSystemToggleTweenerTrack : TweenerTrack<ParticleSystem, bool>
	{
		public override void UpdateComponentValue(float evaluatedTime)
		{
			bool active = Mathf.Approximately(Mathf.LerpUnclamped(_from ? 1f : 0f, _to ? 1f : 0f, evaluatedTime), 1f);
			ToggleParticleSystem(active);
		}

		protected override void ResetDynamicFromValue()
		{
			_from = _component.isPlaying;
		}

		private void ToggleParticleSystem(bool active)
		{
			if (active && !_component.isPlaying)
			{
				_component.Play();
			}
			else if (!active && _component.isPlaying)
			{
				_component.Stop();
			}
		}
	}
}
