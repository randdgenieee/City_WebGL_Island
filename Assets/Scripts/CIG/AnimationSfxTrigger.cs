using UnityEngine;

namespace CIG
{
	public class AnimationSfxTrigger : MonoBehaviour
	{
		private bool _active = true;

		public void SetActive(bool active)
		{
			_active = active;
		}

		private void TriggerSfx(Clip clip)
		{
			if (_active)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(clip);
			}
		}
	}
}
