using UnityEngine.EventSystems;

namespace CIG
{
	public interface IEndPinchHandler : IEventSystemHandler
	{
		void OnEndPinch(PinchEventData pinchEvent);
	}
}
