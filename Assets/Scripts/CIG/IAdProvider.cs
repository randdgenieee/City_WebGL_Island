using System;
using System.Collections;

namespace CIG
{
	public interface IAdProvider
	{
		bool IsReady
		{
			get;
		}

		AdType AdType
		{
			get;
		}

		AdProviderType AdProviderType
		{
			get;
		}

		AdProviderState AdProviderState
		{
			get;
		}

		event AdAvailabilityChangedEventHandler AvailabilityChangedEvent;

		IEnumerator Initialize();

		void Release();

		IEnumerator StartCaching(AdSequenceType adSequenceType);

		bool ShowAd(Action<bool, bool> callback);
	}
}
