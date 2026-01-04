using Tweening;
using UnityEngine;

namespace CIG
{
	public abstract class Popup : MonoBehaviour
	{
		public delegate void OpenCloseTweenerStoppedPlayingHandler(bool closing);

		[SerializeField]
		private GameObject _view;

		[SerializeField]
		private Tweener _openTweener;

		[SerializeField]
		[Tooltip("If null, the 'open tweener' will be played in reverse")]
		private Tweener _closeTweener;

		[SerializeField]
		private DPTarget[] _dpTargets;

		protected PopupManager _popupManager;

		private string _lastScreenName;

		public PopupRequest Request
		{
			get;
			private set;
		}

		public bool IsOpen => Request != null;

		public bool IsInFocus
		{
			get;
			private set;
		}

		public abstract string AnalyticsScreenName
		{
			get;
		}

		private bool CanPlayCloseAnimation
		{
			get
			{
				if (!(_closeTweener != null))
				{
					return _openTweener != null;
				}
				return true;
			}
		}

		public event OpenCloseTweenerStoppedPlayingHandler OpenCloseTweenerStoppedPlayingEvent;

		private void FireOpenCloseTweenerStoppedPlayingEvent(bool closing)
		{
			this.OpenCloseTweenerStoppedPlayingEvent?.Invoke(closing);
		}

		protected virtual void OnDestroy()
		{
			Request = null;
			if (_openTweener != null)
			{
				_openTweener.FinishedPlaying -= OnOpenTweenerFinishedPlaying;
			}
			if (_closeTweener != null)
			{
				_closeTweener.FinishedPlaying -= OnCloseTweenerFinishedPlaying;
			}
			PopScreenView();
		}

		public virtual void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			_popupManager = model.Game.PopupManager;
			_view.SetActive(value: false);
			if (_openTweener != null)
			{
				_openTweener.FinishedPlaying += OnOpenTweenerFinishedPlaying;
			}
			if (_closeTweener != null)
			{
				_closeTweener.FinishedPlaying += OnCloseTweenerFinishedPlaying;
			}
			int i = 0;
			for (int num = _dpTargets.Length; i < num; i++)
			{
				_dpTargets[i].Initialize(canvasScaler);
			}
		}

		public virtual void Open(PopupRequest request)
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PopupOpen);
			Request = request;
			_view.SetActive(value: true);
			SetFocus(isInFocus: true);
			if (_openTweener != null)
			{
				_openTweener.StopAndReset();
				_openTweener.Play();
			}
		}

		public virtual void Close(bool instant)
		{
			PopScreenView();
			SetFocus(isInFocus: false);
			if (!instant && CanPlayCloseAnimation)
			{
				PlayCloseAnimation();
			}
			else
			{
				Closed();
			}
		}

		public void PushScreenView()
		{
			string analyticsScreenName = AnalyticsScreenName;
			if (_lastScreenName != analyticsScreenName)
			{
				ScreenView.UpdateScreenView(_lastScreenName, analyticsScreenName);
				_lastScreenName = analyticsScreenName;
			}
		}

		public void Refresh(PopupRequest request)
		{
			Close(instant: true);
			Open(request);
		}

		public void SetFocus(bool isInFocus)
		{
			if (isInFocus != IsInFocus)
			{
				IsInFocus = isInFocus;
				OnFocusChanged();
			}
		}

		public virtual void OnCloseClicked()
		{
			if (IsOpen)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PopupClose);
				_popupManager.ClosePopup(Request, !CanPlayCloseAnimation);
			}
		}

		public void OnModalBackgroundClicked()
		{
			if (IsOpen && Request.Dismissable)
			{
				OnCloseClicked();
			}
		}

		protected T GetRequest<T>() where T : PopupRequest
		{
			return (T)Request;
		}

		protected virtual void Opened()
		{
		}

		protected virtual void Closed()
		{
			_view.SetActive(value: false);
			Request = null;
		}

		protected void PopScreenView()
		{
			if (!string.IsNullOrEmpty(_lastScreenName))
			{
				ScreenView.PopScreenView(_lastScreenName);
				_lastScreenName = null;
			}
		}

		protected virtual void OnFocusChanged()
		{
		}

		private void PlayCloseAnimation()
		{
			if (_closeTweener != null)
			{
				_closeTweener.StopAndReset();
				_closeTweener.Play();
			}
			else if (_openTweener != null)
			{
				_openTweener.StopAndReset(resetToEnd: true);
				_openTweener.PlayReverse();
			}
		}

		private void OnOpenTweenerFinishedPlaying(Tweener tweener)
		{
			if (tweener.IsPlaybackReversed)
			{
				Closed();
			}
			else
			{
				Opened();
			}
			FireOpenCloseTweenerStoppedPlayingEvent(tweener.IsPlaybackReversed);
		}

		private void OnCloseTweenerFinishedPlaying(Tweener tweener)
		{
			Closed();
			FireOpenCloseTweenerStoppedPlayingEvent(closing: true);
		}
	}
}
