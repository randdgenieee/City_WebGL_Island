using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIGMigrator
{
	public class GameMigratorVersion4 : IMigrator
	{
		private const string XPKey = "XP";

		private const string GameStateKey = "GameState";

		private const string LevelKey = "Level";

		private const string EarnedBalanceKey = "EarnedBalance";

		private static readonly int[] OldXPPerLevel = new int[100]
		{
			0,
			132,
			212,
			292,
			372,
			452,
			532,
			612,
			692,
			772,
			935,
			965,
			1335,
			1600,
			3600,
			6600,
			10000,
			14000,
			19000,
			24000,
			30000,
			41000,
			50000,
			59000,
			82000,
			94000,
			120000,
			130000,
			150000,
			180000,
			190000,
			210000,
			260000,
			300000,
			340000,
			380000,
			410000,
			460000,
			520000,
			570000,
			620000,
			660000,
			710000,
			760000,
			810000,
			870000,
			920000,
			1000000,
			1100000,
			1200000,
			1300000,
			1500000,
			1500000,
			1700000,
			1800000,
			1900000,
			2000000,
			2100000,
			2200000,
			2400000,
			2500000,
			2600000,
			2800000,
			2900000,
			3000000,
			3200000,
			3300000,
			3500000,
			3600000,
			3700000,
			3900000,
			4000000,
			4200000,
			4400000,
			4900000,
			5000000,
			5200000,
			5400000,
			5900000,
			6400000,
			6800000,
			7100000,
			7400000,
			8000000,
			8300000,
			8700000,
			9100000,
			9500000,
			10000000,
			11000000,
			11000000,
			12000000,
			12000000,
			13000000,
			13000000,
			14000000,
			15000000,
			15000000,
			16000000,
			16000000
		};

		private static readonly int[] NewXPPerLevel = new int[100]
		{
			0,
			250,
			500,
			1000,
			1500,
			2000,
			2500,
			3000,
			4000,
			5000,
			8800,
			24000,
			29000,
			34000,
			39000,
			49000,
			62000,
			86000,
			110000,
			130000,
			170000,
			210000,
			260000,
			340000,
			420000,
			510000,
			620000,
			750000,
			900000,
			1100000,
			1200000,
			1400000,
			1700000,
			2000000,
			2300000,
			2600000,
			3000000,
			3400000,
			3800000,
			4400000,
			4900000,
			5500000,
			6100000,
			6800000,
			7500000,
			8200000,
			9100000,
			9900000,
			11000000,
			12000000,
			13000000,
			14000000,
			16000000,
			17000000,
			19000000,
			20000000,
			22000000,
			24000000,
			26000000,
			28000000,
			30000000,
			32000000,
			35000000,
			37000000,
			40000000,
			42000000,
			45000000,
			48000000,
			51000000,
			54000000,
			57000000,
			61000000,
			64000000,
			68000000,
			72000000,
			76000000,
			81000000,
			85000000,
			90000000,
			95000000,
			100000000,
			110000000,
			110000000,
			120000000,
			130000000,
			130000000,
			140000000,
			150000000,
			160000000,
			160000000,
			170000000,
			180000000,
			190000000,
			200000000,
			210000000,
			220000000,
			230000000,
			240000000,
			260000000,
			270000000
		};

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			if (!storageRoot.TryGetValue("GameState", out object value) || !(value is Dictionary<string, object>))
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
			object value3;
			if (!dictionary.TryGetValue("Level", out object value2) || !(value2 is int) || !dictionary.TryGetValue("EarnedBalance", out value3) || !(value3 is Dictionary<string, object>))
			{
				return;
			}
			int num = (int)value2;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)value3;
			if (dictionary2.TryGetValue("XP", out object value4) && value4 is decimal)
			{
				decimal d = (decimal)value4;
				int num2 = NewXPPerLevel.Length;
				int num3 = NewXPPerLevel[Mathf.Clamp(num - 1, 0, num2 - 1)];
				int num4 = NewXPPerLevel[Mathf.Clamp(num, 0, num2 - 1)];
				if (!(d >= (decimal)num3) || !(d < (decimal)num4))
				{
					int num5 = OldXPPerLevel.Length;
					decimal d2 = OldXPPerLevel[Mathf.Clamp(num - 1, 0, num5 - 1)];
					decimal num6 = (decimal)OldXPPerLevel[Mathf.Clamp(num, 0, num5 - 1)] - d2;
					decimal d3 = (num6 == decimal.Zero) ? decimal.One : Math.Max(decimal.Zero, Math.Min(decimal.One, (d - d2) / num6));
					dictionary2["XP"] = (decimal)num3 + (decimal)(num4 - num3) * d3;
				}
			}
		}
	}
}
