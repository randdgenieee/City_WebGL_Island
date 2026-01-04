using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class TreasureChest
	{
		private readonly GameState _gameState;

		public TreasureChestProperties Properties
		{
			get;
		}

		public bool CanAfford => _gameState.CanAfford(Properties.Cost);

		public ILocalizedString Name
		{
			get;
		}

		public TreasureChest(GameState gameState, TreasureChestProperties properties)
		{
			_gameState = gameState;
			Properties = properties;
			Name = GetName(Properties.TreasureChestType);
		}

		private static ILocalizedString GetName(TreasureChestType type)
		{
			switch (type)
			{
			case TreasureChestType.Gold:
				return Localization.Key("chest_gold");
			case TreasureChestType.Platinum:
				return Localization.Key("chest_diamond");
			case TreasureChestType.Silver:
				return Localization.Key("chest_silver");
			case TreasureChestType.Wooden:
				return Localization.Key("chest_free");
			default:
				UnityEngine.Debug.LogError($"No localization available for chest type '{type}'.");
				return Localization.EmptyLocalizedString;
			}
		}
	}
}
