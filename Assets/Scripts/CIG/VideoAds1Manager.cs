using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public sealed class VideoAds1Manager : BaseVideoAdsManager<VideoAds1Properties>
	{
		private readonly PopupManager _popupManager;

		public VideoAds1Manager(StorageDictionary storage, AdProviderPool adProviderPool, CriticalProcesses criticalProcesses, PopupManager popupManager, RoutineRunner routineRunner, VideoAds1Properties properties)
			: base(storage, adProviderPool, routineRunner, GetAdWaterfall(adProviderPool, criticalProcesses), properties)
		{
			_popupManager = popupManager;
		}

		public void WatchVideoForKeys()
		{
			GenericPopupRequest request = new GenericPopupRequest("watch_key_video_confirm").SetTexts(Localization.Key("watch_video"), Localization.Format(Localization.Key("watch_video_popup_text_simple"), Localization.Key("free_keys"))).SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.Keys)).SetGreenButton(Localization.Key("ok"), null, OnWatchVideoConfirmed, SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.VideoSmallest))
				.SetRedCancelButton();
			_popupManager.RequestPopup(request);
		}

		private static AdWaterfall GetAdWaterfall(AdProviderPool adProviderPool, CriticalProcesses criticalProcesses)
		{
			AdProviderType[] adProviderSequence = new AdProviderType[1]
			{
				AdProviderType.AdMobVideo
			};
			return new AdWaterfall(adProviderPool, criticalProcesses, adProviderSequence);
		}

		private void OnWatchVideoConfirmed()
		{
			ShowAd(OnVideoWatched, VideoSource.FreeKeys);
		}

		private void OnVideoWatched(bool success, bool clicked)
		{
			if (success)
			{
				Currency currency = (Random.value <= _properties.GoldKeysChance) ? Currency.GoldKeyCurrency(Random.Range(_properties.MinGoldKeys, _properties.MaxGoldKeys + 1)) : Currency.SilverKeyCurrency(Random.Range(_properties.MinSilverKeys, _properties.MaxSilverKeys + 1));
				_popupManager.RequestPopup(new ReceiveRewardPopupRequest(new Reward(currency), enqueue: false));
			}
		}
	}
}
