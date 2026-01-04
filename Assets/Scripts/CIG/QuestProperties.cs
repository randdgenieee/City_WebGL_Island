namespace CIG
{
	public abstract class QuestProperties : BaseProperties
	{
		private const string IdentifierKey = "identifier";

		private const string QuestTypeKey = "questType";

		[BalanceProperty("identifier")]
		public string Identifier
		{
			get;
			private set;
		}

		[BalanceProperty("questType")]
		public string QuestType
		{
			get;
			private set;
		}

		protected QuestProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Identifier = GetProperty("identifier", "unknown");
			QuestType = GetProperty("questType", "unknown");
		}
	}
}
