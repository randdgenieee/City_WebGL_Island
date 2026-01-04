using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	public sealed class PopupManager
	{
		public delegate void CurrentPopupChangedEventHandler(PopupRequest request);

		public delegate void PopupOpenedEventHandler(PopupRequest request);

		public delegate void PopupClosedEventHandler(PopupRequest request, bool instant);

		private readonly List<PopupRequest> _popupQueue = new List<PopupRequest>();

		public List<PopupRequest> OpenedPopupRequests
		{
			get;
		} = new List<PopupRequest>();


		public bool IsShowingPopup => OpenedPopupRequests.Count > 0;

		public PopupRequest TopPopup
		{
			get
			{
				if (IsShowingPopup)
				{
					return OpenedPopupRequests.Last();
				}
				return null;
			}
		}

		public event CurrentPopupChangedEventHandler CurrentPopupChangedEvent;

		public event PopupOpenedEventHandler PopupOpenedEvent;

		public event PopupClosedEventHandler PopupClosedEvent;

		private void FireCurrentPopupChangedEvent(PopupRequest request)
		{
			this.CurrentPopupChangedEvent?.Invoke(request);
		}

		private void FirePopupOpenedEvent(PopupRequest request)
		{
			this.PopupOpenedEvent?.Invoke(request);
		}

		private void FirePopupClosedEvent(PopupRequest request, bool instant)
		{
			this.PopupClosedEvent?.Invoke(request, instant);
		}

		public void RequestPopup(PopupRequest request)
		{
			if (!IsShowingPopup || !request.Enqueue)
			{
				OpenPopupNow(request);
			}
			else if (request.FirstInQueue)
			{
				_popupQueue.Insert(0, request);
			}
			else
			{
				_popupQueue.Add(request);
			}
		}

		public void ClosePopup(PopupRequest request, bool instant)
		{
			OpenedPopupRequests.Remove(request);
			FirePopupClosedEvent(request, instant);
			TryOpenNextPopup();
		}

		public void ClearAllPopups()
		{
			_popupQueue.Clear();
			OpenedPopupRequests.Clear();
		}

		public void CloseAllPopups(bool instant)
		{
			_popupQueue.Clear();
			CloseAllOpenPopups(instant);
		}

		public void CloseAllOpenPopups(bool instant)
		{
			List<PopupRequest> list = new List<PopupRequest>(OpenedPopupRequests);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				ClosePopup(list[i], instant);
			}
		}

		public bool IsPopupRequested(PopupRequest request)
		{
			if (!_popupQueue.Contains(request))
			{
				return OpenedPopupRequests.Contains(request);
			}
			return true;
		}

		public bool IsShowingPopupOfType<T>() where T : Popup
		{
			int i = 0;
			for (int count = OpenedPopupRequests.Count; i < count; i++)
			{
				if (OpenedPopupRequests[i].PopupType == typeof(T))
				{
					return true;
				}
			}
			return false;
		}

		private void OpenPopupNow(PopupRequest request)
		{
			if (!request.IsValid)
			{
				TryOpenNextPopup();
				return;
			}
			OpenedPopupRequests.Add(request);
			FirePopupOpenedEvent(request);
			FireCurrentPopupChangedEvent(request);
		}

		private void TryOpenNextPopup()
		{
			if (OpenedPopupRequests.Count > 0)
			{
				PopupRequest request = OpenedPopupRequests.Last();
				FireCurrentPopupChangedEvent(request);
			}
			if (_popupQueue.Count > 0 && OpenedPopupRequests.Count <= 0)
			{
				PopupRequest request2 = _popupQueue[0];
				_popupQueue.RemoveAt(0);
				OpenPopupNow(request2);
			}
		}
	}
}
