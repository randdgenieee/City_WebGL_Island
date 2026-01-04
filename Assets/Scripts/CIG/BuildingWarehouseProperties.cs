using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("buildingWarehouse", true)]
	public class BuildingWarehouseProperties : BaseProperties
	{
		private const string PricesKey = "slotPrices";

		[BalanceProperty("slotPrices")]
		public List<int> Prices
		{
			get;
			private set;
		}

		public BuildingWarehouseProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Prices = GetProperty("slotPrices", new List<int>());
		}
	}
}
