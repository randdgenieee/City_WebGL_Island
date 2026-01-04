using UnityEngine;

namespace CIG
{
	public class XPWalkerBalloon : WalkerBalloon
	{
		private readonly WebService _webService;

		public override bool IsAvailable
		{
			get
			{
				if (base.IsAvailable)
				{
					return !_gameState.ReachedMaxLevel;
				}
				return false;
			}
		}

		public XPWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner, WebService webService)
			: base(properties, gameState, popupManager, routineRunner)
		{
			_webService = webService;
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			int num = Mathf.Max(1, _gameState.Level / 10);
			Currency currency = Currency.XPCurrency((decimal)Random.Range(2 * num, 5 * num) * _webService.Multipliers.GetMultiplier(MultiplierType.XP));
			_gameState.EarnCurrencies(currency, CurrenciesEarnedReason.WalkerBalloon, new FlyingCurrenciesData(this));
			FireCurrenciesCollectedEvent(currency, Clip.CollectXP);
			Analytics.WalkerRewardBalloonClicked(_properties.BalloonType.ToString(), currency);
		}
	}
}
