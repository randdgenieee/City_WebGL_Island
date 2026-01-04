using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	[BalancePropertyClass("island", false)]
	public class IslandProperties : BaseProperties
	{
		private const string IslandKey = "id";

		private const string CashCostKey = "unlockCostCash";

		private const string GoldCostKey = "unlockCostGold";

		private const string SurfaceTypesKey = "surfaceTypes";

		[BalanceProperty("id", ParseType = typeof(int))]
		public IslandId IslandId
		{
			get;
		}

		[BalanceProperty("unlockCostCash", ParseType = typeof(decimal))]
		public Currency CashCost
		{
			get;
		}

		[BalanceProperty("unlockCostGold", ParseType = typeof(decimal))]
		public Currency GoldCost
		{
			get;
		}

		[BalanceProperty("surfaceTypes", ParseType = typeof(List<string>))]
		public List<SurfaceTypeInfo> SurfaceTypes
		{
			get;
		}

		public IslandProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			IslandId = (IslandId)GetProperty("id", -1);
			CashCost = Currency.CashCurrency(GetProperty("unlockCostCash", decimal.Zero));
			GoldCost = Currency.GoldCurrency(GetProperty("unlockCostGold", decimal.Zero));
			SurfaceTypes = ParseSurfaceTypes(GetProperty("surfaceTypes", new List<string>()));
		}

		private List<SurfaceTypeInfo> ParseSurfaceTypes(List<string> surfaceTypesString)
		{
			List<SurfaceTypeInfo> list = new List<SurfaceTypeInfo>();
			if (surfaceTypesString.Count % 2 == 0)
			{
				int i = 0;
				for (int count = surfaceTypesString.Count; i < count; i += 2)
				{
					if (SurfaceTypeInfo.TryParse(surfaceTypesString[i], surfaceTypesString[i + 1], out SurfaceTypeInfo result))
					{
						list.Add(result);
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning($"Cannot parse string '{surfaceTypesString}' to '{typeof(List<SurfaceTypeInfo>)}', does not contains an even amount of entries.");
			}
			return list;
		}
	}
}
