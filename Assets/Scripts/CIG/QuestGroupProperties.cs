using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("questGroup", false)]
	public class QuestGroupProperties : BaseProperties
	{
		private const string IdentifiersKey = "identifiers";

		[BalanceProperty("identifiers")]
		public List<string> Identifiers
		{
			get;
			private set;
		}

		public QuestGroupProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Identifiers = GetProperty("identifiers", new List<string>());
		}
	}
}
