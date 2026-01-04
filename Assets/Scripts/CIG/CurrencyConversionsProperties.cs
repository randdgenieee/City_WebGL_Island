using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("currencyConversions", true)]
	public class CurrencyConversionsProperties : BaseProperties
	{
		private const string UnlockLevelKey = "unlockLevel";

		private const string EnabledKey = "enabled";

		public List<CurrencyConversionProperties> CurrencyConversionProperties
		{
			get;
			private set;
		}

		[BalanceProperty("unlockLevel")]
		public int UnlockLevel
		{
			get;
			private set;
		}

		[BalanceProperty("enabled")]
		public bool Enabled
		{
			get;
			private set;
		}

		public CurrencyConversionsProperties(PropertiesDictionary propsDict, string baseKey, List<CurrencyConversionProperties> currencyConversionProperties)
			: base(propsDict, baseKey)
		{
			CurrencyConversionProperties = currencyConversionProperties;
			UnlockLevel = GetProperty("unlockLevel", 15);
			Enabled = GetProperty("enabled", defaultValue: true);
		}
	}
}
