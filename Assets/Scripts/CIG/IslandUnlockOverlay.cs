using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class IslandUnlockOverlay : Overlay
	{
		[SerializeField]
		private LocalizedText _goldAmountText;

		[SerializeField]
		private LocalizedText _cashAmountText;

		[SerializeField]
		private LocalizedText _durationText;

		private GameStats _gameStats;

		private WorldMap _worldMap;

		private IslandId _islandId;

		private Currency _cashCost;

		private Currency _goldCost;

		private int _duration;

		public void Initialize(GameStats gameStats, WorldMap worldMap, IslandId islandId, Currency cashCost, Currency goldCost, int duration)
		{
			_gameStats = gameStats;
			_worldMap = worldMap;
			_islandId = islandId;
			_cashCost = cashCost;
			_goldCost = goldCost;
			_duration = duration;
			_cashAmountText.LocalizedString = Localization.Integer(_cashCost.Value);
			_goldAmountText.LocalizedString = Localization.Integer(_goldCost.Value);
			_durationText.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(duration), hideSecondPartWhenZero: true);
		}

		public void OverrideData(Currency cashCost, Currency goldCost, int duration)
		{
			_cashCost = cashCost;
			_goldCost = goldCost;
			_duration = duration;
			_cashAmountText.LocalizedString = Localization.Integer(_cashCost.Value);
			_goldAmountText.LocalizedString = Localization.Integer(_goldCost.Value);
			_durationText.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(duration), hideSecondPartWhenZero: true);
		}

		public void OnUnlockWithCashClicked()
		{
			_worldMap.Airship.StartTravelling(_islandId, _duration, _cashCost, CurrenciesSpentReason.IslandUnlock, delegate(bool success)
			{
				if (success)
				{
					AnalyticsLog(_cashCost);
					GameEvents.Invoke(new UnemiShouldCloseEvent(this));
				}
			});
		}

		public void OnUnlockWithGoldClicked()
		{
			_worldMap.Airship.InstantTravel(_islandId, _goldCost, CurrenciesSpentReason.IslandUnlock, delegate(bool success)
			{
				if (success)
				{
					AnalyticsLog(_goldCost);
					GameEvents.Invoke(new UnemiShouldCloseEvent(this));
				}
			});
		}

		private void AnalyticsLog(Currency spent)
		{
			_gameStats.IslandPurchased(_islandId.ToString(), spent);
		}
	}
}
