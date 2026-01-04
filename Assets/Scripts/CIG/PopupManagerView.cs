using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public sealed class PopupManagerView : MonoBehaviour
	{
		public delegate void PopupShownEventHandler(Popup popup);

		public delegate void PopupHiddenEventHandler(Popup popup);

		[SerializeField]
		private List<Popup> _popups;

		[SerializeField]
		private ModalBackground _modalBackground;

		[SerializeField]
		private HUDRegionUpdater _hudRegionUpdater;

		[SerializeField]
		private CIGCanvasScaler _canvasScaler;

		private readonly Dictionary<Type, Popup> _popupsDict = new Dictionary<Type, Popup>();

		private readonly List<PopupRequest> _openedPopupRequests = new List<PopupRequest>();

		public PopupManager PopupManager
		{
			get;
			private set;
		}

		public bool IsShowingPopup => _openedPopupRequests.Count > 0;

		public Popup TopPopup
		{
			get
			{
				if (IsShowingPopup)
				{
					return GetPopup(_openedPopupRequests[_openedPopupRequests.Count - 1]);
				}
				return null;
			}
		}

		public event PopupShownEventHandler PopupShownEvent;

		public event PopupHiddenEventHandler PopupHiddenEvent;

		private void FirePopupShownEvent(Popup popup)
		{
			this.PopupShownEvent?.Invoke(popup);
		}

		private void FirePopupHiddenEvent(Popup popup)
		{
			this.PopupHiddenEvent?.Invoke(popup);
		}

		public void Initialize(PopupManager popupManager, Model model)
		{
			PopupManager = popupManager;
			_modalBackground.Initialize();
			int i = 0;
			for (int count = _popups.Count; i < count; i++)
			{
				Popup popup = _popups[i];
				popup.Initialize(model, _canvasScaler);
				_popupsDict.Add(popup.GetType(), popup);
			}
			PopupManager.CurrentPopupChangedEvent += OnCurrentPopupChanged;
			PopupManager.PopupOpenedEvent += OnPopupOpened;
			PopupManager.PopupClosedEvent += OnPopupClosed;
			int j = 0;
			for (int count2 = PopupManager.OpenedPopupRequests.Count; j < count2; j++)
			{
				OnCurrentPopupChanged(PopupManager.OpenedPopupRequests[j]);
			}
		}

		private void OnDestroy()
		{
			if (PopupManager != null)
			{
				PopupManager.CurrentPopupChangedEvent -= OnCurrentPopupChanged;
				PopupManager.PopupOpenedEvent -= OnPopupOpened;
				PopupManager.PopupClosedEvent -= OnPopupClosed;
				PopupManager.ClearAllPopups();
				PopupManager = null;
			}
		}

		public List<T> GetPopups<T>() where T : Popup
		{
			List<T> list = new List<T>();
			int i = 0;
			for (int count = _popups.Count; i < count; i++)
			{
				T item;
				if ((item = (_popups[i] as T)) != null)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public T GetPopup<T>() where T : Popup
		{
			Popup popup = GetPopup(typeof(T));
			T result;
			if ((result = (popup as T)) != null)
			{
				return result;
			}
			UnityEngine.Debug.LogError("Requested popup is not of type " + typeof(T).Name + " but is " + ((popup == null) ? "null" : popup.GetType().Name));
			return null;
		}

		public bool IsShowingPopupOfType<T>() where T : Popup
		{
			int i = 0;
			for (int count = _openedPopupRequests.Count; i < count; i++)
			{
				if (_openedPopupRequests[i].PopupType == typeof(T))
				{
					return true;
				}
			}
			return false;
		}

		private Popup GetPopup(PopupRequest request)
		{
			return GetPopup(request.PopupType);
		}

		private Popup GetPopup(Type popupType)
		{
			if (_popupsDict.TryGetValue(popupType, out Popup value))
			{
				return value;
			}
			UnityEngine.Debug.LogError("Requested popup cannot be found: " + popupType.Name);
			return null;
		}

		private void OnCurrentPopupChanged(PopupRequest request)
		{
			Popup popup = GetPopup(request);
			if (!(popup != null))
			{
				return;
			}
			if (!_openedPopupRequests.Contains(request))
			{
				if (request.ShowModalBackground)
				{
					_modalBackground.Show(popup);
					_modalBackground.transform.SetAsLastSibling();
				}
				if (request.HiddenHudRegion != 0)
				{
					_hudRegionUpdater.RequestHide(popup, request.HiddenHudRegion);
				}
				_openedPopupRequests.Add(request);
			}
			if (popup.IsOpen)
			{
				if (popup.Request != request)
				{
					popup.Refresh(request);
					popup.PushScreenView();
					FirePopupShownEvent(popup);
				}
			}
			else
			{
				popup.Open(request);
				popup.PushScreenView();
				FirePopupShownEvent(popup);
			}
			popup.transform.SetAsLastSibling();
		}

		private void OnPopupOpened(PopupRequest request)
		{
			Popup topPopup = TopPopup;
			if (topPopup != null)
			{
				topPopup.SetFocus(isInFocus: false);
			}
		}

		private void OnPopupClosed(PopupRequest request, bool instant)
		{
			Popup popup = GetPopup(request);
			if (popup != null && (popup.IsOpen || !request.IsValid) && _openedPopupRequests.Contains(request))
			{
				if (request.ShowModalBackground)
				{
					_modalBackground.Hide(popup);
				}
				if (request.HiddenHudRegion != 0)
				{
					_hudRegionUpdater.RequestShow(popup);
				}
				if (popup.IsOpen)
				{
					popup.Close(instant);
				}
				_openedPopupRequests.Remove(request);
				Popup topPopup = TopPopup;
				if (topPopup != null)
				{
					topPopup.SetFocus(isInFocus: true);
				}
				FirePopupHiddenEvent(popup);
			}
		}
	}
}
