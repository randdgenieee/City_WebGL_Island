using UnityEngine.EventSystems;

namespace CIG
{
	public interface IBeginPinchHandler : IEventSystemHandler
	{
		void OnBeginPinch(PinchEventData pinchEvent);
	}
}
