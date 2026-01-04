namespace CIG
{
	[BalancePropertyClass("startBalance", false)]
	public class StartBalanceProperties : BaseProperties
	{
		private const string StartBalanceKey = "startBalance";

		private const string IdentifierKey = "identifier";

		[BalanceProperty("startBalance", Overridable = false)]
		public Currencies StartBalance
		{
			get;
		}

		[BalanceProperty("identifier", Overridable = false)]
		public string Identifier
		{
			get;
		}

		public StartBalanceProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			StartBalance = GetProperty("startBalance", new Currencies());
			Identifier = GetProperty("identifier", "unknown");
		}
	}
}
