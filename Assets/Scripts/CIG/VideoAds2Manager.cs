namespace CIG
{
	public sealed class VideoAds2Manager : BaseVideoAdsManager<VideoAds2Properties>
	{
		public VideoAds2Manager(StorageDictionary storage, AdProviderPool adProviderPool, CriticalProcesses criticalProcesses, RoutineRunner routineRunner, VideoAds2Properties properties)
			: base(storage, adProviderPool, routineRunner, GetAdWaterfall(adProviderPool, criticalProcesses), properties)
		{
		}

		private static AdWaterfall GetAdWaterfall(AdProviderPool adProviderPool, CriticalProcesses criticalProcesses)
		{
			AdProviderType[] adProviderSequence = new AdProviderType[1]
			{
				AdProviderType.AdMobVideo
			};
			return new AdWaterfall(adProviderPool, criticalProcesses, adProviderSequence);
		}
	}
}
