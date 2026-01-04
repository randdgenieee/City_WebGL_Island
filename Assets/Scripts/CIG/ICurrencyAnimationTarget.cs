namespace CIG
{
	public interface ICurrencyAnimationTarget
	{
		void FlyingCurrencyStartedPlaying();

		void FlyingCurrencyFinishedPlaying(Currency earnedCurrency, bool animateHudElement = true);
	}
}
