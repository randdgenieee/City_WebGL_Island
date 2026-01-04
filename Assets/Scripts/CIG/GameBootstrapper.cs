using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GameBootstrapper : MonoBehaviour
	{
		[SerializeField]
		[Header("View")]
		private HUDView _hudView;

		[SerializeField]
		private VisitingHUDView _visitingHudView;

		[SerializeField]
		private HUDController _hudController;

		[SerializeField]
		private CurrencyAnimator _currencyAnimator;

		[SerializeField]
		private PurchaseHandler _purchaseHandler;

		[SerializeField]
		private SessionRestarter _sessionRestarter;

		[SerializeField]
		private PopupManagerView _popupManagerView;

		[SerializeField]
		private WorldMapView _worldMapView;

		[SerializeField]
		private IslandsManagerView _islandsManagerView;

		[SerializeField]
		private TutorialManagerView _tutorialManagerView;

		[SerializeField]
		private RatingRequesterView _ratingRequesterView;

		[SerializeField]
		private ServerSyncer _serverSyncer;

		[SerializeField]
		private TimingView _timingView;

		[SerializeField]
		private GameSceneCloudSaveResolver _cloudSaveResolver;

		[SerializeField]
		private AutoSaver _autoSaver;

		[SerializeField]
		private GameSceneLoader _gameSceneLoader;

		[SerializeField]
		private AudioManager _audioManager;

		[SerializeField]
		private GameQuitter _gameQuitter;

		private Model _model;

		private void Start()
		{
			GameSceneRequest gameSceneRequest;
			if ((gameSceneRequest = (Loader.LastSceneRequest as GameSceneRequest)) == null)
			{
				UnityEngine.Debug.LogError("Scene was not loaded using Scene Loader!");
				return;
			}
			_model = gameSceneRequest.Model;
			_audioManager.Initialize(_model.Device.Settings);
			_timingView.Initialize(_model.Game.Timing);
			_islandsManagerView.Initialize(_model.Game.GameStats, _model.Game.GameState, _model.Game.IslandsManager, _model.Game.ExpansionCostManager, _model.Game.LocalNotificationManager, _model.Game.BuildingWarehouseManager, _model.Game.VideoAds1Manager, _model.Game.VideoAds2Manager, _model.Game.CityAdvisor, _model.Game.TreasureChestManager, _model.Game.TutorialManager, _model.Game.CurrencyConversionManager, _model.Game.BuildingsManager, _model.Game.CraneManager, _model.Game.SaleManager, _model.Game.IslandVisitingManager, _model.Game.IslandVisitingCurrencyManager, _model.Game.WorldMap, _model.Game.PopupManager, _model.GameServer.WebService, _model.Game.Timing, _model.Game.Properties, _model.Game.FishingEvent, _model.Game.GameRoutineRunner);
			_worldMapView.Initialize(_model.Game.GameStats, _model.Game.Timing, _model.Game.PopupManager, _islandsManagerView, _model.Game.WorldMap, _model.Game.Properties.AllIslandProperties);
			_currencyAnimator.Initialize(_model.Game.GameState, _model.Game.BuildingWarehouseManager, _model.Game.CraneManager, _model.Game.Timing);
			_hudController.Initialize(_model.Game.IslandsManager);
			_hudView.Initialize(_model.Game.GameState, _model.Game.GameStats, _model.Game.PopupManager, _model.Game.WorldMap, _model.Game.BuildingWarehouseManager, _model.Game.SaleManager, _model.Game.BuildingsManager, _model.Game.IslandsManager, _model.Game.CraneManager, _model.Game.QuestsManager, _model.Game.VideoAds2Manager, _model.Game.TreasureChestManager, _model.Game.ArcadeManager, _model.Game.FishingEvent, _model.Game.CraneOfferManager, _model.Game.Properties, _model.Game.FlyingStartDealManager, _model.Game.FriendsManager);
			_visitingHudView.Initialize(_model.Game.PopupManager, _model.Game.IslandsManager, _model.Game.IslandVisitingManager, _model.Game.WorldMap, _model.Game.LikeRegistrar);
			_popupManagerView.Initialize(_model.Game.PopupManager, _model);
			_gameSceneLoader.Initialize(_worldMapView);
			_ratingRequesterView.Initialize(_model.Game.RatingRequester, _model.Game.PopupManager, _worldMapView);
			_sessionRestarter.Initialize(_model, _gameSceneLoader);
			_purchaseHandler.Initialize(_model.GameServer.IAPStore, _model.Game.PopupManager, _model.Game.CriticalProcesses, _model.Game.GameStats, _model.Game.CraneManager, _model.Game.GameState, _model.Game.TreasureChestManager, _model.Game.BuildingWarehouseManager, _model.Game.CraneOfferManager, _model.Game.FlyingStartDealManager);
			List<IAPPopup> popups = _popupManagerView.GetPopups<IAPPopup>();
			int i = 0;
			for (int count = popups.Count; i < count; i++)
			{
				popups[i].Initialize(_purchaseHandler);
			}
			_popupManagerView.GetPopup<LinkConfirmPopup>().Initialize(_gameSceneLoader);
			_popupManagerView.GetPopup<SettingsPopup>().Initialize(_gameSceneLoader);
			_gameQuitter.Initialize(_popupManagerView);
			_tutorialManagerView.Initialize(_model.Game.TutorialManager, _popupManagerView, _worldMapView, _islandsManagerView, _hudView);
			_serverSyncer.Initialize(_model.GameServer, _model.Game);
			_autoSaver.Initialize();
			_cloudSaveResolver.Initialize(_model, _tutorialManagerView, _gameSceneLoader);
		}

		private void OnApplicationPause(bool paused)
		{
			_model.Game.SessionManager.ApplicationPause(paused);
			_autoSaver.ApplicationPause(paused);
			_model.Game.EngagementTracker.ApplicationPause(paused);
			if (_sessionRestarter.ApplicationPause(paused))
			{
				_model.Release();
			}
		}

		private void OnApplicationQuit()
		{
			_model.Game.SessionManager.ApplicationQuit();
			_autoSaver.ApplicationQuit();
			_model.Game.EngagementTracker.ApplicationQuit();
			_model.Release();
			_model = null;
		}
	}
}
