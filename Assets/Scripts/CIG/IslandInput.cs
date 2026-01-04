using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class IslandInput : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IBeginPinchHandler, IPinchHandler, IEndPinchHandler
	{
		public delegate void PointerClickEventHandler(PointerEventData eventData);

		public delegate void PointerDownEventHandler(PointerEventData eventData);

		public delegate void PointerUpEventHandler(PointerEventData eventData);

		public delegate void BeginPinchEventHandler(PinchEventData pinchEvent);

		public delegate void PinchEventHandler(PinchEventData pinchEvent);

		public delegate void EndPinchEventHandler(PinchEventData pinchEvent);

		private const string GridObjectsLayerName = "GridObjects";

		[SerializeField]
		private Physics2DRaycaster _islandRaycaster;

		[SerializeField]
		private EventSystem _eventSystem;

		private int _gridObjectsLayerID;

		private List<object> _requesters = new List<object>();

		public EventSystem EventSystem => _eventSystem;

		public event PointerClickEventHandler PointerClickEvent;

		public event PointerDownEventHandler PointerDownEvent;

		public event PointerUpEventHandler PointerUpEvent;

		public event BeginPinchEventHandler BeginPinchEvent;

		public event PinchEventHandler PinchEvent;

		public event EndPinchEventHandler EndPinchEvent;

		public void Initialize()
		{
			_gridObjectsLayerID = LayerMask.NameToLayer("GridObjects");
		}

		public void PopDisableIslandInteractionRequest(object requester)
		{
			_requesters.Remove(requester);
			if (_requesters.Count == 0)
			{
				_islandRaycaster.eventMask = -1;
			}
		}

		public void PushDisableIslandInteractionRequest(object requester)
		{
			_requesters.Add(requester);
			_islandRaycaster.eventMask = ~(1 << _gridObjectsLayerID);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (this.PointerClickEvent != null)
			{
				this.PointerClickEvent(eventData);
			}
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.PointerDownEvent != null)
			{
				this.PointerDownEvent(eventData);
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.PointerUpEvent != null)
			{
				this.PointerUpEvent(eventData);
			}
		}

		void IBeginPinchHandler.OnBeginPinch(PinchEventData pinchEvent)
		{
			if (this.BeginPinchEvent != null)
			{
				this.BeginPinchEvent(pinchEvent);
			}
		}

		void IPinchHandler.OnPinch(PinchEventData pinchEvent)
		{
			if (this.PinchEvent != null)
			{
				this.PinchEvent(pinchEvent);
			}
		}

		void IEndPinchHandler.OnEndPinch(PinchEventData pinchEvent)
		{
			if (this.EndPinchEvent != null)
			{
				this.EndPinchEvent(pinchEvent);
			}
		}
	}
}
