using UnityEngine.EventSystems;

namespace CIG
{
	public interface IPinchHandler : IEventSystemHandler
	{
		void OnPinch(PinchEventData pinchEvent);
	}
}
