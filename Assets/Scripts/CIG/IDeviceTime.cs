namespace CIG
{
	public interface IDeviceTime
	{
		long GetTimeSinceBoot();

		bool SupportsRestartCheck();

		bool HasRestartedSinceLastCheck(long lastTimeSinceBoot);
	}
}
