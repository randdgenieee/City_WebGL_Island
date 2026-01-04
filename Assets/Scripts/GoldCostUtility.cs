using CIG;
using System;
using UnityEngine;

public static class GoldCostUtility
{
	private struct GoldCostBucket
	{
		public decimal MaxSeconds
		{
			get;
		}

		public decimal GoldCost
		{
			get;
		}

		public GoldCostBucket(decimal maxHours, decimal goldCost)
		{
			MaxSeconds = maxHours * 3600m;
			GoldCost = goldCost;
		}
	}

	private static readonly GoldCostBucket[] GoldCosts = new GoldCostBucket[30]
	{
		new GoldCostBucket(decimal.One, decimal.One),
		new GoldCostBucket(2m, 2m),
		new GoldCostBucket(3m, 3m),
		new GoldCostBucket(5m, 4m),
		new GoldCostBucket(8m, 5m),
		new GoldCostBucket(12m, 6m),
		new GoldCostBucket(17m, 7m),
		new GoldCostBucket(23m, 8m),
		new GoldCostBucket(30m, 9m),
		new GoldCostBucket(38m, 10m),
		new GoldCostBucket(47m, 11m),
		new GoldCostBucket(57m, 12m),
		new GoldCostBucket(68m, 13m),
		new GoldCostBucket(80m, 14m),
		new GoldCostBucket(93m, 15m),
		new GoldCostBucket(107m, 16m),
		new GoldCostBucket(122m, 17m),
		new GoldCostBucket(138m, 18m),
		new GoldCostBucket(155m, 19m),
		new GoldCostBucket(173m, 20m),
		new GoldCostBucket(192m, 21m),
		new GoldCostBucket(212m, 22m),
		new GoldCostBucket(233m, 23m),
		new GoldCostBucket(255m, 24m),
		new GoldCostBucket(278m, 25m),
		new GoldCostBucket(302m, 26m),
		new GoldCostBucket(327m, 27m),
		new GoldCostBucket(353m, 28m),
		new GoldCostBucket(380m, 29m),
		new GoldCostBucket(999999999m, 30m)
	};

	public static decimal GetSpeedupCostGoldForSeconds(Multipliers multipliers, long seconds)
	{
		decimal multiplier = multipliers.GetMultiplier(MultiplierType.UpspeedGoldCost);
		int i = 0;
		for (int num = GoldCosts.Length; i < num; i++)
		{
			GoldCostBucket goldCostBucket = GoldCosts[i];
			if ((decimal)seconds <= goldCostBucket.MaxSeconds)
			{
				return Math.Max(decimal.One, Math.Round(goldCostBucket.GoldCost * multiplier));
			}
		}
		return Math.Max(decimal.One, Math.Round(GoldCosts[GoldCosts.Length - 1].GoldCost * multiplier));
	}

	public static decimal GetGoldCostForMissingCash(decimal currentCash, decimal cashCost, int maxGoldCost)
	{
		if (cashCost <= decimal.Zero)
		{
			return decimal.Zero;
		}
		return Mathf.CeilToInt(Mathf.Clamp01(1f - (float)(currentCash / cashCost)) * (float)maxGoldCost);
	}
}
