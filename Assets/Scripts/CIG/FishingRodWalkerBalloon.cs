namespace CIG
{
	public class FishingRodWalkerBalloon : WalkerBalloon
	{
		public FishingRodWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
			: base(properties, gameState, popupManager, routineRunner)
		{
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			Currency currency = new Currency("FishingRod", decimal.One);
			_gameState.EarnCurrencies(currency, CurrenciesEarnedReason.WalkerBalloon, new FlyingCurrenciesData(this));
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.CollectCoinsCash);
			Analytics.WalkerRewardBalloonClicked(_properties.GroupType.ToString(), currency);
		}
	}
}
