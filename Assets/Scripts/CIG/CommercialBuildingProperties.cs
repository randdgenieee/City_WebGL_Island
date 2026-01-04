using CIG.Translation;
using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	[BalancePropertyClass("commercial", false)]
	[BalanceHiddenProperty("profitXP", typeof(decimal))]
	[BalanceHiddenProperty("profitCash", typeof(List<decimal>))]
	[BalanceSortedArrayProperties("profitCash", true)]
	[BalanceSortedArrayProperties("jobs", true)]
	[BalanceEquallySizedProperties(new string[]
	{
		"jobs",
		"profitCash"
	})]
	public class CommercialBuildingProperties : BuildingProperties
	{
		private const string ProfitDurationSecondsKey = "profitDurationSeconds";

		private const string ProfitPerLevelKey = "profitCash";

		private const string ProfitXpKey = "profitXP";

		private const string EmployeesPerLevelKey = "jobs";

		private const string CurrencyConversionMultiplierKey = "conversionMultiplier";

		[BalanceProperty("profitDurationSeconds")]
		public int ProfitDurationSeconds
		{
			get;
		}

		public List<Currencies> ProfitPerLevel
		{
			get;
		}

		[BalanceProperty("jobs")]
		public List<int> EmployeesPerLevel
		{
			get;
		}

		[BalanceProperty("conversionMultiplier")]
		public float CurrencyConversionMultiplier
		{
			get;
		}

		public override ILocalizedString CategoryName => Localization.Key("shop_commercial");

		public CommercialBuildingProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			decimal xpProfit = GetProperty("profitXP", decimal.Zero);
			ProfitPerLevel = ListExtensions.ConvertAll(GetProperty("profitCash", new List<decimal>()), (decimal cash) => new Currencies(new Currency("Cash", cash), new Currency("XP", xpProfit)));
			ProfitDurationSeconds = GetProperty("profitDurationSeconds", 0);
			EmployeesPerLevel = GetProperty("jobs", new List<int>());
			CurrencyConversionMultiplier = GetProperty("conversionMultiplier", 1f);
		}

		public override List<BuildingProperty> GetShownProperties(bool preview)
		{
			List<BuildingProperty> shownProperties = base.GetShownProperties(preview);
			shownProperties.Add(BuildingProperty.Profit);
			if (!preview)
			{
				shownProperties.Add(BuildingProperty.ProfitXp);
			}
			shownProperties.Add(BuildingProperty.Employees);
			shownProperties.Add(BuildingProperty.TimeUntilProfit);
			return shownProperties;
		}
	}
}
