namespace CIG
{
	[BalancePropertyClass("oneTimeOfferTreasureChest", true)]
	public class OneTimeOfferTreasureChestProperties : OneTimeOfferBaseProperties
	{
		private const string IAPCooldownSecondsKey = "iapCooldownSeconds";

		[BalanceProperty("iapCooldownSeconds")]
		public int IAPCooldownSeconds
		{
			get;
			private set;
		}

		public OneTimeOfferTreasureChestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			IAPCooldownSeconds = GetProperty("iapCooldownSeconds", 259200);
		}
	}
}
