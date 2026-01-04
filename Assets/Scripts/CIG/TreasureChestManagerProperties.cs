using System.Collections.Generic;
using System.Globalization;

namespace CIG
{
	[BalancePropertyClass("treasureChests", true)]
	[BalanceHiddenProperty("silverKeyValues", typeof(List<string>))]
	[BalanceHiddenProperty("goldKeyValues", typeof(List<string>))]
	[BalanceListCountProperties("silverKeyValues", true)]
	[BalanceListCountProperties("goldKeyValues", true)]
	public class TreasureChestManagerProperties : BaseProperties
	{
		private const string SilverKeyValuesKey = "silverKeyValues";

		private const string GoldKeyValuesKey = "goldKeyValues";

		public List<TreasureChestProperties> TreasureChestProperties
		{
			get;
		}

		public decimal SilverKeyValueCash
		{
			get;
		}

		public decimal SilverKeyValueGold
		{
			get;
		}

		public decimal SilverKeyValueGoldKeys
		{
			get;
		}

		public decimal SilverKeyValueTokens
		{
			get;
		}

		public decimal SilverKeyValueLevelUp
		{
			get;
		}

		public decimal GoldKeyValueCash
		{
			get;
		}

		public decimal GoldKeyValueGold
		{
			get;
		}

		public decimal GoldKeyValueTokens
		{
			get;
		}

		public decimal GoldKeyValueLevelUp
		{
			get;
		}

		public decimal GoldKeyValueCrane
		{
			get;
		}

		public TreasureChestManagerProperties(PropertiesDictionary propsDict, string baseKey, List<TreasureChestProperties> treasureChestProperties)
			: base(propsDict, baseKey)
		{
			TreasureChestProperties = treasureChestProperties;
			Dictionary<string, decimal> dict = BaseProperties.ParseKeyValue(GetProperty("silverKeyValues", new List<string>()), delegate(string input, out decimal output)
			{
				return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out output);
			});
			SilverKeyValueCash = GetValue(dict, "SilverKey");
			SilverKeyValueGold = GetValue(dict, "Gold");
			SilverKeyValueGoldKeys = GetValue(dict, "GoldKey");
			SilverKeyValueTokens = GetValue(dict, "Token");
			SilverKeyValueLevelUp = GetValue(dict, "LevelUp");
			Dictionary<string, decimal> dict2 = BaseProperties.ParseKeyValue(GetProperty("goldKeyValues", new List<string>()), delegate(string input, out decimal output)
			{
				return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out output);
			});
			GoldKeyValueCash = GetValue(dict2, "Cash");
			GoldKeyValueGold = GetValue(dict2, "Gold");
			GoldKeyValueTokens = GetValue(dict2, "Token");
			GoldKeyValueLevelUp = GetValue(dict2, "LevelUp");
			GoldKeyValueCrane = GetValue(dict2, "Crane");
		}

		private static decimal GetValue(Dictionary<string, decimal> dict, string name)
		{
			if (!dict.TryGetValue(name, out decimal value))
			{
				return decimal.Zero;
			}
			return value;
		}
	}
}
