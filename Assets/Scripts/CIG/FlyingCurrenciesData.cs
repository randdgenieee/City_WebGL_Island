using UnityEngine;

namespace CIG
{
	public class FlyingCurrenciesData
	{
		public const float DelayBetweenSameSourceFlyingCurrencies = 0.1f;

		private const float MaxTimeCapToMakeFlyingCurrencies = 3f;

		private const int MaxAmountOfFlyingCurrencies = 30;

		public object EarnSource
		{
			get;
		}

		public int AmountOfFlyingCurrencies
		{
			get;
		}

		public FlyingCurrenciesData(object earnSource = null, int amountOfFlyingCurrencies = 1)
		{
			EarnSource = earnSource;
			AmountOfFlyingCurrencies = Mathf.Clamp(amountOfFlyingCurrencies, 0, 30);
		}
	}
}
