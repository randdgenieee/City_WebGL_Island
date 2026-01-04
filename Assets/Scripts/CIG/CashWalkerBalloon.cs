using UnityEngine;

namespace CIG
{
	public class CashWalkerBalloon : WalkerBalloon
	{
		public CashWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			int num = Mathf.Max(1, _gameState.Level / 10);
			Currency currency = Currency.CashCurrency(Random.Range(20 * num, 50 * num));
			_gameState.EarnCurrencies(currency, CurrenciesEarnedReason.WalkerBalloon, new FlyingCurrenciesData(this));
			Analytics.WalkerRewardBalloonClicked(_properties.BalloonType.ToString(), currency);
			FireCurrenciesCollectedEvent(currency, Clip.CollectCoinsCash);
		}
	}
}
