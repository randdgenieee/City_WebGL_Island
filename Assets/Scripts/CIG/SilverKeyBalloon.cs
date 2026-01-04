using UnityEngine;

namespace CIG
{
	public class SilverKeyBalloon : WalkerBalloon
	{
		private readonly int _minKeys;

		private readonly int _maxKeys;

		public SilverKeyBalloon(SilverKeyBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
			_minKeys = properties.MinSilverKeyAmount;
			_maxKeys = properties.MaxSilverKeyAmount;
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			Currency currency = Currency.SilverKeyCurrency(Random.Range(_minKeys, _maxKeys + 1));
			_gameState.EarnCurrencies(currency, CurrenciesEarnedReason.WalkerBalloon, new FlyingCurrenciesData(this));
			Analytics.WalkerRewardBalloonClicked(_properties.BalloonType.ToString(), currency);
			FireCurrenciesCollectedEvent(currency, Clip.CollectCoinsCash);
		}
	}
}
