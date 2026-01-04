using Tweening;
using UnityEngine;

namespace CIG
{
	public class InteractableButtonTweener : InteractableButtonComponent
	{
		[SerializeField]
		private Tweener _tweener;

		protected override void OnInteractable()
		{
			_tweener.PlayIfStopped();
		}

		protected override void OnNonInteractable()
		{
			_tweener.StopIfPlaying();
		}
	}
}
