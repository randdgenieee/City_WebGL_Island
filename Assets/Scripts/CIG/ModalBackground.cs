using System.Collections.Generic;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class ModalBackground : MonoBehaviour
	{
		[SerializeField]
		private Tweener _showTweener;

		private readonly List<Popup> _popups = new List<Popup>();

		private void OnDestroy()
		{
			if (_showTweener != null)
			{
				_showTweener.FinishedPlaying -= OnShowTweenerFinishedPlaying;
			}
		}

		public void Initialize()
		{
			base.gameObject.SetActive(value: false);
			_showTweener.FinishedPlaying += OnShowTweenerFinishedPlaying;
		}

		public void Show(Popup popup)
		{
			_popups.Insert(0, popup);
			UpdateOverlay();
			if (!base.gameObject.activeInHierarchy || (_showTweener.IsPlaying && _showTweener.IsPlaybackReversed))
			{
				base.gameObject.SetActive(value: true);
				_showTweener.StopAndReset();
				_showTweener.Play();
			}
		}

		public void Hide(Popup popup)
		{
			_popups.Remove(popup);
			UpdateOverlay();
			if (_popups.Count <= 0)
			{
				_showTweener.StopAndReset(resetToEnd: true);
				_showTweener.PlayReverse();
			}
		}

		public void OnBackgroundClicked()
		{
			if (_popups.Count > 0)
			{
				_popups[0].OnModalBackgroundClicked();
			}
		}

		private void UpdateOverlay()
		{
			if (_popups.Count > 0)
			{
				base.transform.SetAsLastSibling();
				_popups[0].transform.SetAsLastSibling();
			}
		}

		private void OnShowTweenerFinishedPlaying(Tweener tweener)
		{
			if (tweener.IsPlaybackReversed)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
