using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("expansionCost", true)]
	public class ExpansionProperties : BaseProperties
	{
		private const string CostsKey = "cashCost";

		[BalanceProperty("cashCost")]
		public List<int> Costs
		{
			get;
			private set;
		}

		public ExpansionProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Costs = GetProperty("cashCost", new List<int>());
		}
	}
}
