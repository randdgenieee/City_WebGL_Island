using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GameStateProperties
	{
		public const string DefaultBalanceIdentifier = "Default";

		public List<StartBalanceProperties> StartBalanceProperties
		{
			get;
		}

		public List<LevelProperties> LevelProperties
		{
			get;
		}

		public GameStateProperties(List<LevelProperties> allLevelProperties, List<StartBalanceProperties> startBalanceProperties)
		{
			LevelProperties = allLevelProperties;
			StartBalanceProperties = startBalanceProperties;
		}

		public string GetRandomStartBalanceABGroup()
		{
			return (from b in StartBalanceProperties
				where b.Identifier != "Default"
				select b).PickRandom().Identifier;
		}

		public Currencies GetStartBalance(string identifier)
		{
			StartBalanceProperties startBalanceProperties = StartBalanceProperties.Find((StartBalanceProperties p) => p.Identifier == identifier);
			if (startBalanceProperties != null)
			{
				return startBalanceProperties.StartBalance;
			}
			Currencies startBalance = StartBalanceProperties[0].StartBalance;
			UnityEngine.Debug.LogError($"Unknown startbalance Identifier: {identifier}, returning first element: {startBalance}");
			return startBalance;
		}
	}
}
