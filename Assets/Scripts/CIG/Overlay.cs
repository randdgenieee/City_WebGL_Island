using Tweening;
using UnityEngine;

namespace CIG
{
	public class Overlay : MonoBehaviour
	{
		[SerializeField]
		protected Tweener _tweener;

		public virtual void Show()
		{
			TryStopTweener();
			if (_tweener == null)
			{
				OnOverlayShown();
				return;
			}
			base.gameObject.SetActive(value: true);
			_tweener.FinishedPlaying += OnShowTweenerFinishedPlaying;
			_tweener.Play();
		}

		public virtual void Remove()
		{
			TryStopTweener();
			if (_tweener == null || !base.gameObject.activeInHierarchy)
			{
				OnOverlayRemoved();
				return;
			}
			base.transform.SetParent(base.transform.parent);
			_tweener.FinishedPlaying += OnRemoveTweenerFinishedPlaying;
			_tweener.PlayReverse();
		}

		protected virtual void TryStopTweener()
		{
			if (_tweener != null)
			{
				_tweener.FinishedPlaying -= OnShowTweenerFinishedPlaying;
				_tweener.FinishedPlaying -= OnRemoveTweenerFinishedPlaying;
				_tweener.StopAndReset();
			}
		}

		protected virtual void OnOverlayShown()
		{
		}

		protected virtual void OnOverlayRemoved()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnShowTweenerFinishedPlaying(Tweener tweener)
		{
			tweener.FinishedPlaying -= OnShowTweenerFinishedPlaying;
			OnOverlayShown();
		}

		private void OnRemoveTweenerFinishedPlaying(Tweener tweener)
		{
			tweener.FinishedPlaying -= OnRemoveTweenerFinishedPlaying;
			OnOverlayRemoved();
		}
	}
}
