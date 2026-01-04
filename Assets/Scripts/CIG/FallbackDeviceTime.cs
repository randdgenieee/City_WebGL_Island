using UnityEngine;

namespace CIG
{
	public class FallbackDeviceTime : IDeviceTime
	{
		private bool _hasRestarted = true;

		public long GetTimeSinceBoot()
		{
			return (long)Time.realtimeSinceStartup;
		}

		public bool HasRestartedSinceLastCheck(long lastTimeSinceBoot)
		{
			bool hasRestarted = _hasRestarted;
			_hasRestarted = false;
			return hasRestarted;
		}

		public bool SupportsRestartCheck()
		{
			return true;
		}
	}
}
