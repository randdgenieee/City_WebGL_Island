using Tweening;
using UnityEngine;

namespace CIG
{
	public class QuestItemStar : MonoBehaviour
	{
		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		private ParticleSystem _particles;

		private bool _filled;

		public void Initialize(bool filled)
		{
			_filled = filled;
			_tweener.StopAndReset(_filled);
		}

		public void UpdateLook(bool fill)
		{
			if (fill != _filled)
			{
				_filled = fill;
				_tweener.StopAndReset();
				if (_filled)
				{
					_tweener.Play();
					_particles.Play();
					SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.CollectCoinsCash);
				}
			}
		}
	}
}
