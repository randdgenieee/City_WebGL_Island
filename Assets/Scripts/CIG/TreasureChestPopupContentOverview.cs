using CIG.Translation;
using SparkLinq;
using System.Collections;
using System.Collections.Generic;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class TreasureChestPopupContentOverview : MonoBehaviour
	{
		private const float LandmarkImageWidth = 250f;

		private const float BuildingImageWidth = 150f;

		[SerializeField]
		private TMPTextStyleView _header;

		[SerializeField]
		private Tweener _openTweener;

		[Header("Wooden chest")]
		[SerializeField]
		private GameObject _woodenChestGroup;

		[SerializeField]
		private RectTransform _woodenCurrenciesLayout;

		[SerializeField]
		private Tweener _doubleRewardBadgeTweener;

		[Header("Other Chests")]
		[SerializeField]
		private GameObject _otherChestsGroup;

		[Header("Top left")]
		[SerializeField]
		private GameObject _goldStackGroup;

		[SerializeField]
		private LocalizedText _goldStackAmountText;

		[SerializeField]
		private GameObject _landmarkGroup;

		[SerializeField]
		private BuildingImage _landmarkBuildingImage;

		[Header("Top right")]
		[SerializeField]
		private RectTransform _currenciesLayout;

		[SerializeField]
		private AnimatedCurrencyView _currencyPrefab;

		[SerializeField]
		private GameObject _goldCurrencyGroup;

		[SerializeField]
		private LocalizedText _goldCurrencyAmount;

		[SerializeField]
		private GameObject _craneCurrencyGroup;

		[SerializeField]
		private LocalizedText _craneCurrencyAmount;

		[SerializeField]
		private GameObject _tokenCurrencyGroup;

		[SerializeField]
		private LocalizedText _tokenCurrencyAmount;

		[Header("Middle")]
		[SerializeField]
		private BuildingImage _buildingPrefab;

		[SerializeField]
		private GameObject _buildingGroup;

		[SerializeField]
		private RectTransform _buildingParent;

		[Header("Bottom")]
		[SerializeField]
		private GameObject _bottomGroup;

		[SerializeField]
		private GameObject _goldValueGroup;

		[SerializeField]
		private NumberTweenHelper _goldValueAmountText;

		[SerializeField]
		private GameObject _cashValueGroup;

		[SerializeField]
		private NumberTweenHelper _cashValueAmountText;

		[SerializeField]
		private GameObject _rareBuildingsGroup;

		[SerializeField]
		private NumberTweenHelper _rareBuildingsAmountText;

		[SerializeField]
		private GameObject _doubleRewardButton;

		[SerializeField]
		private Tweener _doubleRewardButtonTweener;

		[SerializeField]
		private Tweener _continueButtonTweener;

		private VideoAds3Manager _videoAdsManager;

		private TreasureChestRewards _treasureRewards;

		private readonly List<BuildingImage> _buildingItems = new List<BuildingImage>();

		private readonly List<AnimatedCurrencyView> _currencyItems = new List<AnimatedCurrencyView>();

		private bool _tweenBottomValues;

		private decimal _totalGoldValue;

		private decimal _totalCashValue;

		private decimal _totalRareBuildings;

		private void Awake()
		{
			_openTweener.FinishedPlaying += OnOpenTweenerFinishedPlaying;
		}

		public void Initialize(TreasureChestRewards rewards, VideoAds3Manager videoManager)
		{
			_videoAdsManager = videoManager;
			_treasureRewards = rewards;
			//_header.ApplyStyle(GetHeaderStyle(rewards.TreasureChestType));
			_doubleRewardBadgeTweener.gameObject.SetActive(value: false);
			_treasureRewards.VideoWatchedForDoubleRewardEvent += OnVideoWatchedForDoubleReward;
			if (rewards.TreasureChestType == TreasureChestType.Wooden)
			{
				_woodenChestGroup.SetActive(value: true);
				_otherChestsGroup.SetActive(value: false);
				SetWoodenChestContent(rewards.Reward);
			}
			else
			{
				_otherChestsGroup.SetActive(value: true);
				_woodenChestGroup.SetActive(value: false);
				SetOtherChestsContent(rewards.TreasureChestType, rewards.Reward);
			}
		}

		public void Deinitialize()
		{
			int i = 0;
			for (int count = _buildingItems.Count; i < count; i++)
			{
				UnityEngine.Object.Destroy(_buildingItems[i].gameObject);
			}
			_buildingItems.Clear();
			int j = 0;
			for (int count2 = _currencyItems.Count; j < count2; j++)
			{
				UnityEngine.Object.Destroy(_currencyItems[j].gameObject);
			}
			_currencyItems.Clear();
			if (_treasureRewards != null)
			{
				_treasureRewards.VideoWatchedForDoubleRewardEvent -= OnVideoWatchedForDoubleReward;
			}
		}

		private void OnDestroy()
		{
			if (_openTweener != null)
			{
				_openTweener.FinishedPlaying -= OnOpenTweenerFinishedPlaying;
			}
			if (_treasureRewards != null)
			{
				_treasureRewards.VideoWatchedForDoubleRewardEvent -= OnVideoWatchedForDoubleReward;
			}
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
			if (!active)
			{
				return;
			}
			_openTweener.PlayIfStopped();
			if (_treasureRewards.TreasureChestType == TreasureChestType.Wooden && (_treasureRewards.IsDoubled || _videoAdsManager.IsReady))
			{
				if (_treasureRewards.IsDoubled)
				{
					_doubleRewardButton.SetActive(value: false);
					_doubleRewardBadgeTweener.gameObject.SetActive(value: true);
					_doubleRewardBadgeTweener.Play();
					_continueButtonTweener.Play();
				}
				else if (_videoAdsManager.IsReady)
				{
					_doubleRewardButton.SetActive(value: true);
					_doubleRewardButtonTweener.PlayIfStopped();
					_continueButtonTweener.StopIfPlaying();
				}
			}
			else
			{
				_doubleRewardButton.SetActive(value: false);
				_continueButtonTweener.PlayIfStopped();
				_doubleRewardButtonTweener.StopIfPlaying();
			}
		}

		public void OnWatchVideoForDoubleRewardClicked()
		{
			_treasureRewards.WatchVideoForDoubleReward(_videoAdsManager);
		}

		private void OnVideoWatchedForDoubleReward(bool success)
		{
			_treasureRewards.VideoWatchedForDoubleRewardEvent -= OnVideoWatchedForDoubleReward;
			if (success)
			{
				StartCoroutine(WoodenChestDoubleRewardRoutine());
			}
			else if (!_videoAdsManager.IsReady)
			{
				_doubleRewardButton.SetActive(value: false);
				_doubleRewardButtonTweener.StopIfPlaying();
				_continueButtonTweener.PlayIfStopped();
			}
		}

		private IEnumerator WoodenChestDoubleRewardRoutine()
		{
			_doubleRewardButton.SetActive(value: false);
			_doubleRewardBadgeTweener.gameObject.SetActive(value: true);
			_doubleRewardBadgeTweener.StopAndReset();
			_doubleRewardBadgeTweener.Play();
			yield return new WaitWhile(() => _doubleRewardBadgeTweener.IsPlaying);
			yield return new WaitForSeconds(0.2f);
			for (int i = 0; i < _currencyItems.Count; i++)
			{
				_currencyItems[i].UpdateValue(_currencyItems[i].CurrentValue * 2m);
			}
			_continueButtonTweener.Play();
		}

		private void SetWoodenChestContent(Reward reward)
		{
			SetCurrenciesContent(reward.Currencies, _woodenCurrenciesLayout, 30);
		}

		private void SetOtherChestsContent(TreasureChestType chestType, Reward reward)
		{
			Currencies currencies = new Currencies(reward.Currencies);
			LandmarkBuildingProperties landmarkBuildingProperties = reward.Buildings.Find((BuildingProperties p) => p is LandmarkBuildingProperties) as LandmarkBuildingProperties;
			_landmarkGroup.SetActive(landmarkBuildingProperties != null);
			if (landmarkBuildingProperties != null)
			{
				_landmarkBuildingImage.Initialize(landmarkBuildingProperties, MaterialType.UIClip, 250f);
			}
			decimal value = reward.Currencies.GetValue("Gold");
			bool flag = landmarkBuildingProperties == null && chestType == TreasureChestType.Gold && value > decimal.Zero;
			_goldStackGroup.SetActive(flag);
			if (flag)
			{
				_goldStackAmountText.LocalizedString = Localization.Integer(value);
				currencies.Remove("Gold");
			}
			List<BuildingProperties> list = reward.Buildings.ToList();
			list.Remove(landmarkBuildingProperties);
			int count = list.Count;
			_buildingGroup.gameObject.SetActive(count > 0);
			for (int i = 0; i < count; i++)
			{
				BuildingImage buildingImage = UnityEngine.Object.Instantiate(_buildingPrefab, _buildingParent);
				buildingImage.Initialize(list[i], MaterialType.UIClip, 150f);
				_buildingItems.Add(buildingImage);
			}
			int showingCount = 0;
			ShowSpecialCurrencyGroup(_goldCurrencyGroup, _goldCurrencyAmount, currencies, "Gold", ref showingCount);
			ShowSpecialCurrencyGroup(_craneCurrencyGroup, _craneCurrencyAmount, currencies, "Crane", ref showingCount);
			ShowSpecialCurrencyGroup(_tokenCurrencyGroup, _tokenCurrencyAmount, currencies, "Token", ref showingCount);
			SetCurrenciesContent(currencies, _currenciesLayout, 22);
			if (reward.Buildings.Count > 0)
			{
				_totalGoldValue = reward.Buildings.Sum((BuildingProperties b) => (!b.BaseConstructionCost.IsMatchingName("Gold")) ? decimal.Zero : b.BaseConstructionCost.Value);
				_goldValueGroup.SetActive(_totalGoldValue > decimal.Zero);
				_goldValueAmountText.TweenTo(decimal.Zero, 0f);
				_totalCashValue = reward.Buildings.Sum((BuildingProperties b) => (!b.BaseConstructionCost.IsMatchingName("Cash")) ? decimal.Zero : b.BaseConstructionCost.Value);
				_cashValueGroup.SetActive(_totalCashValue > decimal.Zero);
				_cashValueAmountText.TweenTo(decimal.Zero, 0f);
				_totalRareBuildings = reward.Buildings.Count((BuildingProperties b) => b.BaseConstructionCost.IsMatchingName("Gold"));
				_rareBuildingsGroup.SetActive(_totalRareBuildings > decimal.Zero);
				_rareBuildingsAmountText.TweenTo(decimal.Zero, 0f);
			}
			else
			{
				_totalGoldValue = default(decimal);
				_totalCashValue = default(decimal);
				_totalRareBuildings = default(decimal);
			}
			_tweenBottomValues = (_totalGoldValue > decimal.Zero || _totalCashValue > decimal.Zero || _totalRareBuildings > decimal.Zero);
			_bottomGroup.SetActive(_tweenBottomValues);
		}

		private void SetCurrenciesContent(Currencies currencies, RectTransform parent, int fontSize)
		{
			int i = 0;
			for (int keyCount = currencies.KeyCount; i < keyCount; i++)
			{
				Currency currency = currencies.GetCurrency(i);
				if (currency.Value > decimal.Zero)
				{
					AnimatedCurrencyView animatedCurrencyView = UnityEngine.Object.Instantiate(_currencyPrefab, parent);
					animatedCurrencyView.Initialize(currency);
					animatedCurrencyView.AmountText.TextField.fontSize = fontSize;
					_currencyItems.Add(animatedCurrencyView);
				}
			}
		}

		private TMPTextStyleType GetHeaderStyle(TreasureChestType chestType)
		{
			switch (chestType)
			{
			case TreasureChestType.Wooden:
				return TMPTextStyleType.Wood;
			case TreasureChestType.Silver:
				return TMPTextStyleType.Silver;
			case TreasureChestType.Gold:
				return TMPTextStyleType.Gold;
			case TreasureChestType.Platinum:
				return TMPTextStyleType.Boost;
			default:
				UnityEngine.Debug.LogError(string.Format("Cannot determine header style for {0} {1}", "TreasureChestType", chestType));
				return TMPTextStyleType.None;
			}
		}

		private static void ShowSpecialCurrencyGroup(GameObject currencyGroup, LocalizedText currencyAmountText, Currencies currencies, string currency, ref int showingCount)
		{
			decimal value = currencies.GetValue(currency);
			if (value > decimal.Zero && showingCount < 2)
			{
				currencyGroup.SetActive(value: true);
				currencyAmountText.LocalizedString = Localization.Integer(value);
				currencies.Remove(currency);
				showingCount++;
			}
			else
			{
				currencyGroup.SetActive(value: false);
			}
		}

		private void OnOpenTweenerFinishedPlaying(Tweener tweener)
		{
			if (!tweener.IsPlaybackReversed && _tweenBottomValues)
			{
				_goldValueAmountText.TweenTo(decimal.Zero, _totalGoldValue);
				_cashValueAmountText.TweenTo(decimal.Zero, _totalCashValue);
				_rareBuildingsAmountText.TweenTo(decimal.Zero, _totalRareBuildings);
			}
		}
	}
}
