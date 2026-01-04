using UnityEngine;

namespace CIG
{
	public class WalkerBalloonFactory
	{
		private readonly GameState _gameState;

		private readonly PopupManager _popupManager;

		private readonly CityAdvisor _cityAdvisor;

		private readonly WebService _webService;

		private readonly RoutineRunner _routineRunner;

		public WalkerBalloonFactory(GameState gameState, PopupManager popupManager, RoutineRunner routineRunner, CityAdvisor cityAdvisor, WebService webService)
		{
			_gameState = gameState;
			_popupManager = popupManager;
			_routineRunner = routineRunner;
			_cityAdvisor = cityAdvisor;
			_webService = webService;
		}

		public WalkerBalloon CreateBalloon(WalkerBalloonProperties properties)
		{
			switch (properties.BalloonType)
			{
			case WalkerBalloonType.Upgrade:
				return new UpgradeWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.Community:
				return new CommunityWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.Residential:
				return new ResidentialWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.Unemployed:
				return new UnemployedWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.Cash:
				return new CashWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.XP:
				return new XPWalkerBalloon(properties, _gameState, _popupManager, _routineRunner, _webService);
			case WalkerBalloonType.Advisor:
				return new AdvisorWalkerBalloon(properties, _gameState, _popupManager, _routineRunner, _cityAdvisor);
			case WalkerBalloonType.SilverKey:
				return new SilverKeyBalloon((SilverKeyBalloonProperties)properties, _gameState, _popupManager, _routineRunner);
			case WalkerBalloonType.FishingRod:
				return new FishingRodWalkerBalloon(properties, _gameState, _popupManager, _routineRunner);
			default:
				UnityEngine.Debug.LogErrorFormat("Unknown balloonType: {0}", properties.BalloonType);
				return null;
			}
		}
	}
}
