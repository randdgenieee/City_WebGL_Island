namespace CIG
{
	public class AdvisorWalkerBalloon : WalkerBalloon
	{
		private readonly CityAdvisor _cityAdvisor;

		private AdviceType? _lastAdviceType;

		public override bool IsAvailable
		{
			get
			{
				if (!base.IsAvailable)
				{
					return false;
				}
				_cityAdvisor.UpdateAdvice();
				if (_lastAdviceType.HasValue)
				{
					return _cityAdvisor.AdviceType != _lastAdviceType.Value;
				}
				return true;
			}
		}

		public AdvisorWalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner, CityAdvisor cityAdvisor)
			: base(properties, gameState, popupManager, routineRunner)
		{
			_cityAdvisor = cityAdvisor;
		}

		protected override void OnCollected()
		{
			base.OnCollected();
			_cityAdvisor.Show();
			_lastAdviceType = _cityAdvisor.AdviceType;
			Analytics.WalkerExclamationBalloonClicked(_properties.BalloonType.ToString());
		}
	}
}
