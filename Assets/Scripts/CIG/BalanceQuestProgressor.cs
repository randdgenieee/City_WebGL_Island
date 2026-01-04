namespace CIG
{
	public class BalanceQuestProgressor : QuestProgressor
	{
		private GameState _gameState;

		private readonly string _currency;

		public BalanceQuestProgressor(StorageDictionary storage, GameState gameState, string currency, long startProgress)
			: base(storage, startProgress)
		{
			_gameState = gameState;
			_currency = currency;
			_gameState.BalanceChangedEvent += OnBalanceChanged;
		}

		public override void Release()
		{
			if (_gameState != null)
			{
				_gameState.BalanceChangedEvent -= OnBalanceChanged;
				_gameState = null;
			}
			base.Release();
		}

		private void OnBalanceChanged(Currencies oldbalance, Currencies newbalance, FlyingCurrenciesData flyingCurrenciesData)
		{
			Currencies currencies = newbalance - oldbalance;
			if (currencies.Contains(_currency))
			{
				base.Progress += (long)currencies.GetValue(_currency);
			}
		}
	}
}
