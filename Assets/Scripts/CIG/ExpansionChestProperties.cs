namespace CIG
{
	[BalancePropertyClass("ExpansionChest", true)]
	public class ExpansionChestProperties : GridTileProperties
	{
		private const string MinTokensKey = "minTokens";

		private const string MaxTokensKey = "maxTokens";

		[BalanceProperty("minTokens")]
		public int MinTokens
		{
			get;
			private set;
		}

		[BalanceProperty("maxTokens")]
		public int MaxTokens
		{
			get;
			private set;
		}

		public ExpansionChestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			MinTokens = GetProperty("minTokens", 5);
			MaxTokens = GetProperty("maxTokens", 10);
		}
	}
}
