namespace CIG
{
	public class OneTimeOfferBaseProperties : BaseProperties
	{
		private const string MinimumLevelRequiredKey = "minimumLevelRequired";

		private const string CooldownSecondsKey = "cooldownSeconds";

		private const string EnabledKey = "enabled";

		[BalanceProperty("minimumLevelRequired")]
		public int MinimumLevelRequired
		{
			get;
			private set;
		}

		[BalanceProperty("cooldownSeconds")]
		public int CooldownSeconds
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

		public OneTimeOfferBaseProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			MinimumLevelRequired = GetProperty("minimumLevelRequired", 10);
			CooldownSeconds = GetProperty("cooldownSeconds", 28800);
			Enabled = GetProperty("enabled", defaultValue: true);
		}
	}
}
