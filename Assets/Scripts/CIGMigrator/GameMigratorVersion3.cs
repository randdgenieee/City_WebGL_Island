using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIGMigrator
{
	public class GameMigratorVersion3 : IMigrator
	{
		private const string XPKey = "XP";

		private const string GameStatsKey = "GameStats";

		private const string GlobalProfitPerHourKey = "GlobalProfitPerHour";

		private const string GameStateKey = "GameState";

		private const string LevelKey = "Level";

		private const string EarnedBalanceKey = "EarnedBalance";

		private const float OldFactorMinutesPerLevel = 0.1f;

		private const float OldFactorMinutesPerLevelPerLevel = 0.175f;

		private static readonly long[] OldBaseXPPerLevel = new long[100]
		{
			132L,
			212L,
			292L,
			372L,
			452L,
			532L,
			612L,
			692L,
			772L,
			935L,
			965L,
			1335L,
			1705L,
			2075L,
			2445L,
			2815L,
			3185L,
			3555L,
			3925L,
			4661L,
			4990L,
			5377L,
			5764L,
			6151L,
			6538L,
			6925L,
			7312L,
			7699L,
			8086L,
			8858L,
			10108L,
			10604L,
			11100L,
			11596L,
			12092L,
			12588L,
			13084L,
			13580L,
			14076L,
			15063L,
			17595L,
			17760L,
			17925L,
			18090L,
			18255L,
			18420L,
			18585L,
			18750L,
			18915L,
			19241L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			23644L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			28165L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			31986L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L,
			39723L
		};

		private static readonly int[] NewXPPerLevel = new int[100]
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

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			object value2;
			if (!storageRoot.TryGetValue("GameStats", out object value) || !(value is Dictionary<string, object>) || !storageRoot.TryGetValue("GameState", out value2) || !(value2 is Dictionary<string, object>))
			{
				return;
			}
			Dictionary<string, object> obj = (Dictionary<string, object>)value;
			Dictionary<string, object> dictionary = (Dictionary<string, object>)value2;
			object value5;
			object value4;
			if (!obj.TryGetValue("GlobalProfitPerHour", out object value3) || !(value3 is Dictionary<string, object>) || !dictionary.TryGetValue("Level", out value4) || !(value4 is int) || !dictionary.TryGetValue("EarnedBalance", out value5) || !(value5 is Dictionary<string, object>))
			{
				return;
			}
			Dictionary<string, object> obj2 = (Dictionary<string, object>)value3;
			int num = (int)value4;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)value5;
			object value7;
			if (obj2.TryGetValue("XP", out object value6) && value6 is decimal && dictionary2.TryGetValue("XP", out value7) && value7 is decimal)
			{
				decimal globalProfitPerHourXP = (decimal)value6;
				decimal d = (decimal)value7;
				decimal num2 = default(decimal);
				for (int i = 0; i < num; i++)
				{
					num2 = OldCalculateRequiredXP(i, num2, globalProfitPerHourXP);
				}
				decimal num3 = OldCalculateRequiredXP(num, num2, globalProfitPerHourXP) - num2;
				decimal d2 = (num3 == decimal.Zero) ? decimal.One : Math.Max(decimal.Zero, Math.Min(decimal.One, (d - num2) / num3));
				int num4 = NewXPPerLevel.Length;
				int num5 = NewXPPerLevel[Mathf.Clamp(num - 1, 0, num4 - 1)];
				int num6 = NewXPPerLevel[Mathf.Clamp(num, 0, num4 - 1)];
				dictionary2["XP"] = (decimal)num5 + (decimal)(num6 - num5) * d2;
			}
		}

		private decimal OldCalculateRequiredXP(int level, decimal previousLevelXP, decimal globalProfitPerHourXP)
		{
			decimal num;
			if (level <= 0)
			{
				num = 0.0m;
			}
			else if (level == 1)
			{
				num = 200.0m;
			}
			else
			{
				num = ((level >= OldBaseXPPerLevel.Length) ? ((decimal)OldBaseXPPerLevel[OldBaseXPPerLevel.Length - 1]) : ((decimal)OldBaseXPPerLevel[level]));
				if (level > 12 || (int)Math.Round(num) == 0)
				{
					decimal d = (decimal)(0.1f + (float)level * 0.175f);
					num += globalProfitPerHourXP / 60.0m * (decimal)level * d;
				}
			}
			return num + previousLevelXP;
		}
	}
}
