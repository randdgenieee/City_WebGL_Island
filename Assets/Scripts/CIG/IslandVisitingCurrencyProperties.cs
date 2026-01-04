using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("islandVisitingCurrency", true)]
	public class IslandVisitingCurrencyProperties : BaseProperties
	{
		private const string CurrencyKey = "currency";

		private const string AmountRangeKey = "amountRange";

		private const string CollectDurationSecondsKey = "collectDurationSeconds";

		private const string DailyAmountPerFriendKey = "dailyAmountPerFriend";

		private const string DailyAmountTotalKey = "dailyAmountTotal";

		[BalanceProperty("currency")]
		public string Currency
		{
			get;
		}

		[BalanceProperty("amountRange")]
		public List<int> AmountRange
		{
			get;
		}

		[BalanceProperty("collectDurationSeconds")]
		public int CollectDurationSeconds
		{
			get;
		}

		[BalanceProperty("dailyAmountPerFriend")]
		public int DailyAmountPerFriend
		{
			get;
		}

		[BalanceProperty("dailyAmountTotal")]
		public int DailyAmountTotal
		{
			get;
		}

		public IslandVisitingCurrencyProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Currency = GetProperty("currency", "SilverKey");
			AmountRange = GetProperty("amountRange", new List<int>());
			CollectDurationSeconds = GetProperty("collectDurationSeconds", 3600);
			DailyAmountPerFriend = GetProperty("dailyAmountPerFriend", 0);
			DailyAmountTotal = GetProperty("dailyAmountTotal", 0);
		}
	}
}
