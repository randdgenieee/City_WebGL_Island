using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("wheelOfFortune", true)]
	public class WheelOfFortuneProperties : BaseProperties
	{
		private const string NormalPriceKey = "normalPrice";

		private const string PremiumPriceKey = "premiumPrice";

		public List<WheelOfFortuneStreakProperties> Streaks
		{
			get;
		}

		[BalanceProperty("normalPrice")]
		public Currency NormalPrice
		{
			get;
		}

		[BalanceProperty("premiumPrice")]
		public Currency PremiumPrice
		{
			get;
		}

		public WheelOfFortuneProperties(PropertiesDictionary propsDict, string baseKey, List<WheelOfFortuneStreakProperties> streaks)
			: base(propsDict, baseKey)
		{
			Streaks = streaks;
			NormalPrice = GetProperty("normalPrice", Currency.Invalid);
			PremiumPrice = GetProperty("premiumPrice", Currency.Invalid);
		}
	}
}
