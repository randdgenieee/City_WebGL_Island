using System.Collections.Generic;

namespace CIG
{
	public class CurrencyConversionManager
	{
		private readonly GameState _gameState;

		private readonly CurrencyConversionsProperties _properties;

		public bool FeatureEnabled
		{
			get
			{
				if (_properties.Enabled)
				{
					return Unlocked;
				}
				return false;
			}
		}

		private bool Unlocked => _gameState.Level >= _properties.UnlockLevel;

		public List<CurrencyConversionProperties> ConversionProperties => _properties.CurrencyConversionProperties;

		public CurrencyConversionManager(GameState gameState, CurrencyConversionsProperties properties)
		{
			_gameState = gameState;
			_properties = properties;
		}
	}
}
