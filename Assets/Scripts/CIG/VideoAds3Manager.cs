namespace CIG
{
	public sealed class VideoAds3Manager : BaseVideoAdsManager<VideoAds3Properties>
	{
		public VideoAds3Manager(StorageDictionary storage, AdProviderPool adProviderPool, CriticalProcesses criticalProcesses, RoutineRunner routineRunner, VideoAds3Properties properties)
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
