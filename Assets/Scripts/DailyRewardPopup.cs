using CIG;
using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardPopup : Popup
{
	[Serializable]
	public class DailyRewardItemStyle
	{
		[SerializeField]
		private Sprite _panelSprite;

		[SerializeField]
		private Sprite _iconSprite;

		[SerializeField]
		private Sprite _currencySprite;

		public Sprite PanelSprite => _panelSprite;

		public Sprite IconSprite => _iconSprite;

		public Sprite CurrencySprite => _currencySprite;
	}

	[SerializeField]
	private DailyRewardItem _itemPrefab;

	[SerializeField]
	private DailyRewardItemStyle _cashItemStyle;

	[SerializeField]
	private DailyRewardItemStyle _goldItemStyle;

	[SerializeField]
	private DailyRewardItemStyle _xpItemStyle;

	[SerializeField]
	private DailyRewardItemStyle _silverKeyItemStyle;

	[SerializeField]
	private DailyRewardItemStyle _goldKeyItemStyle;

	[SerializeField]
	private DailyRewardItemStyle _tokenItemStyle;

	[SerializeField]
	private Transform _rewardsParent;

	private readonly List<DailyRewardItem> _currentItems = new List<DailyRewardItem>();

	private Currency _todaysReward;

	public override string AnalyticsScreenName => "daily_reward";

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		DailyRewardPopupRequest request2 = GetRequest<DailyRewardPopupRequest>();
		_todaysReward = request2.TodaysReward;
		ClearItems();
		int i = 0;
		for (int count = request2.Streak.Count; i < count; i++)
		{
			Currency dailyReward = (i != request2.DayIndex) ? request2.Streak[i] : _todaysReward;
			if (TryGetItemStyle(dailyReward, out DailyRewardItemStyle itemStyle, out ILocalizedString amount))
			{
				DailyRewardItem.ItemState state = (i == request2.DayIndex) ? DailyRewardItem.ItemState.Active : ((i >= request2.DayIndex) ? DailyRewardItem.ItemState.Inactive : DailyRewardItem.ItemState.Collected);
				DailyRewardItem dailyRewardItem = UnityEngine.Object.Instantiate(_itemPrefab, _rewardsParent);
				dailyRewardItem.Show(itemStyle, i, amount, state, OpenCollectPopup);
				_currentItems.Add(dailyRewardItem);
			}
		}
	}

	private void OpenCollectPopup()
	{
		_popupManager.RequestPopup(new DailyRewardCollectPopupRequest(_todaysReward));
		OnCloseClicked();
	}

	private void ClearItems()
	{
		int i = 0;
		for (int count = _currentItems.Count; i < count; i++)
		{
			UnityEngine.Object.Destroy(_currentItems[i].gameObject);
		}
		_currentItems.Clear();
	}

	private bool TryGetItemStyle(Currency dailyReward, out DailyRewardItemStyle itemStyle, out ILocalizedString amount)
	{
		switch (dailyReward.Name)
		{
		case "Gold":
			itemStyle = _goldItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		case "Cash":
			itemStyle = _cashItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		case "LevelUp":
			itemStyle = _xpItemStyle;
			amount = Localization.Format(Localization.Key("iap.title.levels$n"), Localization.Float(dailyReward.Value, 1, showTrailingZeroes: false));
			return true;
		case "XP":
			itemStyle = _xpItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		case "SilverKey":
			itemStyle = _silverKeyItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		case "GoldKey":
			itemStyle = _goldKeyItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		case "Token":
			itemStyle = _tokenItemStyle;
			amount = Localization.Integer(dailyReward.Value);
			return true;
		default:
			UnityEngine.Debug.LogError("Could not find item style for the given currencies: " + dailyReward.Name);
			itemStyle = null;
			amount = null;
			return false;
		}
	}
}
