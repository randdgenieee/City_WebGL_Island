namespace CIG
{
	[BalancePropertyClass("saleManager", true)]
	public class SaleManagerProperties : BaseProperties
	{
		private const string IgnoreEdwinServerSalesKey = "ignoreEdwinServerSales";

		[BalanceProperty("ignoreEdwinServerSales")]
		public bool IgnoreEdwinServerSales
		{
			get;
			private set;
		}

		public SaleManagerProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			IgnoreEdwinServerSales = GetProperty("ignoreEdwinServerSales", defaultValue: true);
		}
	}
}
