using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class IslandGoToOverlay : Overlay
	{
		[SerializeField]
		private LocalizedText _goldAmountText;

		[SerializeField]
		private LocalizedText _durationText;

		private WorldMap _worldMap;

		private IslandId _islandId;

		private Currency _goldCost;

		private int _duration;

		public void Initialize(WorldMap worldMap, IslandId islandId, int duration)
		{
			_worldMap = worldMap;
			_islandId = islandId;
			_duration = duration;
			_goldCost = _worldMap.Airship.GetGoldCostForTravelDuration(_duration);
			_goldAmountText.LocalizedString = Localization.Integer(_goldCost.Value);
			_durationText.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(duration), hideSecondPartWhenZero: true);
		}

		public void OnCancelClicked()
		{
			Remove();
		}

		public void OnSendClicked()
		{
			_worldMap.Airship.StartTravelling(_islandId, _duration, Currency.CashCurrency(decimal.Zero), CurrenciesSpentReason.AirshipSend, delegate(bool success)
			{
				if (success)
				{
					GameEvents.Invoke(new UnemiShouldCloseEvent(this));
				}
			});
		}

		public void OnSendWithGoldClicked()
		{
			_worldMap.Airship.InstantTravel(_islandId, _goldCost, CurrenciesSpentReason.AirshipSend, delegate(bool success)
			{
				if (success)
				{
					GameEvents.Invoke(new UnemiShouldCloseEvent(this));
				}
			});
		}
	}
}
