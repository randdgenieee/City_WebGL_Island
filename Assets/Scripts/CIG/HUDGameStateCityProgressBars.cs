namespace CIG
{
	public class HUDGameStateCityProgressBars : HUDCityProgressBars
	{
		private GameState _gameState;

		protected override int Happiness => _gameState.GlobalHappiness;

		protected override int Population => _gameState.GlobalPopulation;

		protected override int Housing => _gameState.GlobalHousing;

		protected override int Employees => _gameState.GlobalEmployees;

		protected override int Jobs => _gameState.GlobalJobs;

		public void Initialize(GameState gameState)
		{
			_gameState = gameState;
			_gameState.ValueChangedEvent += OnGameStateValueChanged;
			UpdateHappiness();
			UpdatePopulation();
			UpdateJobs();
		}

		private void OnDestroy()
		{
			if (_gameState != null)
			{
				_gameState.ValueChangedEvent -= OnGameStateValueChanged;
				_gameState = null;
			}
		}

		private void OnGameStateValueChanged(string key, object oldValue, object newValue)
		{
			if (key.Equals("Happiness") || key.Equals("Housing") || key.Equals("GlobalHappiness") || key.Equals("GlobalHousing"))
			{
				UpdateHappiness();
			}
			if (key.Equals("Population") || key.Equals("Housing") || key.Equals("GlobalPopulation") || key.Equals("GlobalHousing"))
			{
				UpdatePopulation();
			}
			if (key.Equals("Employees") || key.Equals("Jobs") || key.Equals("GlobalEmployees") || key.Equals("GlobalJobs"))
			{
				UpdateJobs();
			}
		}
	}
}
