using CIG.Translation;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CIG
{
	public class TutorialManagerView : MonoBehaviour
	{
		[SerializeField]
		private TutorialPointer _tutorialPointer;

		[SerializeField]
		private TutorialDialog _tutorialDialog;

		[SerializeField]
		private Camera _uiCamera;

		[SerializeField]
		private GameObject _quitButton;

		[SerializeField]
		private WorldMapNewIslandTutorialView _worldMapNewIslandTutorialView;

		[SerializeField]
		private BuildCommercialTutorialView _buildCommercialTutorialView;

		[SerializeField]
		private BuildCommunityTutorialView _buildCommunityTutorialView;

		[SerializeField]
		private BuildResidentialTutorialView _buildResidentialTutorialView;

		[SerializeField]
		private BuildRoadTutorialView _buildRoadTutorialView;

		[SerializeField]
		private CurrencyConversionTutorialView _currencyConversionTutorialView;

		[SerializeField]
		private InsufficientCranesTutorialView _insufficientCranesTutorialView;

		[SerializeField]
		private StartTutorialView _startTutorialView;

		[SerializeField]
		private TreasureChestTutorialView _treasureChestTutorialView;

		[SerializeField]
		private WarehouseTutorialView _warehouseTutorialView;

		[SerializeField]
		private WorldMapStartTutorialView _worldMapStartTutorialView;

		[SerializeField]
		private WorldMapUnlockIslandTutorialView _worldMapUnlockIslandTutorialView;

		[SerializeField]
		private WorldMapWrapUpTutorialView _worldMapWrapUpTutorialView;

		[SerializeField]
		private FishingEventTutorialView _fishingEventTutorialView;

		[SerializeField]
		private ExpansionTutorialView _expansionTutorialView;

		[SerializeField]
		private BigWarehouseTutorialView _bigWarehouseTutorialView;

		private TutorialManager _tutorialManager;

		private PopupManagerView _popupManagerView;

		private WorldMapView _worldMapView;

		private IslandsManagerView _islandsManagerView;

		private HUDView _hudView;

		private BuyExpansionPopup _buyExpansionPopup;

		private BuildingPopup _buildingPopup;

		private BuildingWarehousePopup _buildingWarehousePopup;

		private bool _quitTutorialPopupOpen;

		private bool PopupBlocksQuitButton
		{
			get
			{
				if (!_quitTutorialPopupOpen && !_popupManagerView.IsShowingPopupOfType<BuildConfirmPopup>() && !_popupManagerView.IsShowingPopupOfType<RoadSelectionPopup>())
				{
					return _popupManagerView.IsShowingPopupOfType<LevelUpPopup>();
				}
				return true;
			}
		}

		public TutorialPointer Pointer => _tutorialPointer;

		public void Initialize(TutorialManager tutorialManager, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDView hudView)
		{
			_tutorialManager = tutorialManager;
			_popupManagerView = popupManagerView;
			_worldMapView = worldMapView;
			_islandsManagerView = islandsManagerView;
			_hudView = hudView;
			_buyExpansionPopup = _popupManagerView.GetPopup<BuyExpansionPopup>();
			_buildingPopup = _popupManagerView.GetPopup<BuildingPopup>();
			_buildingWarehousePopup = _popupManagerView.GetPopup<BuildingWarehousePopup>();
			_tutorialDialog.Initialize();
			_tutorialPointer.Initialize(_uiCamera, _worldMapView);
			_tutorialManager.TutorialStartedEvent += OnTutorialStarted;
			_tutorialManager.TutorialStoppedEvent += OnTutorialStopped;
			_tutorialManager.TutorialFinishedEvent += OnTutorialFinished;
			_popupManagerView.PopupShownEvent += OnPopupShown;
			_popupManagerView.PopupHiddenEvent += OnPopupHidden;
			if (_tutorialManager.ActiveTutorial != null)
			{
				OnTutorialStarted(_tutorialManager.ActiveTutorial);
			}
			_worldMapNewIslandTutorialView.Initialize(_worldMapView, _popupManagerView, _tutorialPointer);
		}

		private void OnDestroy()
		{
			if (_tutorialManager != null)
			{
				_tutorialManager.TutorialStartedEvent -= OnTutorialStarted;
				_tutorialManager.TutorialStoppedEvent -= OnTutorialStopped;
				_tutorialManager.TutorialFinishedEvent -= OnTutorialFinished;
				_tutorialManager = null;
			}
			if (_popupManagerView != null)
			{
				_popupManagerView.PopupShownEvent -= OnPopupShown;
				_popupManagerView.PopupHiddenEvent -= OnPopupHidden;
				_popupManagerView = null;
			}
		}

		public void OnQuitTutorialClicked()
		{
			_quitTutorialPopupOpen = true;
			_quitButton.SetActive(value: false);
			GenericPopupRequest request = new GenericPopupRequest("tutorial_quit_confirm").SetTexts(Localization.Key("quit_tutorial"), Localization.Key("tutorial_quit_confirm")).SetDismissable(dismissable: true, _003COnQuitTutorialClicked_003Eg__DismissAction_007C35_0).SetGreenOkButton(_tutorialManager.QuitTutorial)
				.SetRedCancelButton(_003COnQuitTutorialClicked_003Eg__DismissAction_007C35_0);
			_popupManagerView.PopupManager.RequestPopup(request);
		}

		private void DeinitializeTutorial(Tutorial tutorial)
		{
			switch (tutorial.TutorialType)
			{
			case TutorialType.Start:
				_startTutorialView.Deinitialize();
				break;
			case TutorialType.BuildResidential:
				_buildResidentialTutorialView.Deinitialize();
				break;
			case TutorialType.BuildRoad:
				_buildRoadTutorialView.Deinitialize();
				break;
			case TutorialType.BuildCommercial:
				_buildCommercialTutorialView.Deinitialize();
				break;
			case TutorialType.BuildCommunity:
				_buildCommunityTutorialView.Deinitialize();
				break;
			case TutorialType.WorldMapStart:
				_worldMapStartTutorialView.Deinitialize();
				break;
			case TutorialType.WorldMapUnlockIsland:
				_worldMapUnlockIslandTutorialView.Deinitialize();
				break;
			case TutorialType.WorldMapWrapUp:
				_worldMapWrapUpTutorialView.Deinitialize();
				break;
			case TutorialType.InsufficientCranes:
				_insufficientCranesTutorialView.Deinitialize();
				break;
			case TutorialType.Warehouse:
				_warehouseTutorialView.Deinitialize();
				break;
			case TutorialType.CurrencyConversion:
				_currencyConversionTutorialView.Deinitialize();
				break;
			case TutorialType.TreasureChest:
				_treasureChestTutorialView.Deinitialize();
				break;
			case TutorialType.FishingEvent:
				_fishingEventTutorialView.Deinitialize();
				break;
			case TutorialType.Expansion:
				_expansionTutorialView.Deinitialize();
				break;
			case TutorialType.BigWarehouse:
				_bigWarehouseTutorialView.Deinitialize();
				break;
			default:
				UnityEngine.Debug.LogWarning("There is no view to deinitialize for " + tutorial.TutorialType);
				break;
			}
		}

		private void OnPopupShown(Popup popup)
		{
			_quitButton.SetActive(_tutorialManager.ActiveTutorial != null && _tutorialManager.ActiveTutorial.CanQuit && !PopupBlocksQuitButton);
		}

		private void OnPopupHidden(Popup popup)
		{
			_quitButton.SetActive(_tutorialManager.ActiveTutorial != null && _tutorialManager.ActiveTutorial.CanQuit && !PopupBlocksQuitButton);
		}

		private void OnTutorialStarted(Tutorial tutorial)
		{
			switch (tutorial.TutorialType)
			{
			case TutorialType.Start:
				_startTutorialView.Initialize((StartTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.BuildResidential:
				_buildResidentialTutorialView.Initialize((BuildResidentialTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.ShopButton);
				break;
			case TutorialType.BuildRoad:
				_buildRoadTutorialView.Initialize((BuildRoadTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.RoadsButtonMaskTransform);
				break;
			case TutorialType.BuildCommercial:
				_buildCommercialTutorialView.Initialize((BuildCommercialTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.ShopButton);
				break;
			case TutorialType.BuildCommunity:
				_buildCommunityTutorialView.Initialize((BuildCommunityTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.ShopButton);
				break;
			case TutorialType.WorldMapStart:
				_worldMapStartTutorialView.Initialize((WorldMapStartTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.WorldMapUnlockIsland:
				_worldMapUnlockIslandTutorialView.Initialize((WorldMapUnlockIslandTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.MapButton);
				break;
			case TutorialType.WorldMapWrapUp:
				_worldMapWrapUpTutorialView.Initialize((WorldMapWrapUpTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.InsufficientCranes:
				_insufficientCranesTutorialView.Initialize((InsufficientCranesTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.Warehouse:
				_warehouseTutorialView.Initialize((WarehouseTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.WarehouseButton);
				break;
			case TutorialType.CurrencyConversion:
				_currencyConversionTutorialView.Initialize((CurrencyConversionTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.TreasureChest:
				_treasureChestTutorialView.Initialize((TreasureChestTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.ShopButton);
				break;
			case TutorialType.FishingEvent:
				_fishingEventTutorialView.Initialize((FishingEventTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView);
				break;
			case TutorialType.Expansion:
				_expansionTutorialView.Initialize((ExpansionTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _buyExpansionPopup);
				break;
			case TutorialType.BigWarehouse:
				_bigWarehouseTutorialView.Initialize((BigWarehouseTutorial)tutorial, _tutorialDialog, _tutorialPointer, _popupManagerView, _worldMapView, _islandsManagerView, _hudView.WarehouseButton, _buildingPopup, _buildingWarehousePopup);
				break;
			default:
				UnityEngine.Debug.LogWarning("There is no view to initialize for " + tutorial.TutorialType);
				break;
			}
			_quitButton.SetActive(tutorial.CanQuit && !PopupBlocksQuitButton);
		}

		private void OnTutorialStopped(Tutorial tutorial)
		{
			DeinitializeTutorial(tutorial);
			_quitButton.SetActive(value: false);
		}

		private void OnTutorialFinished(Tutorial tutorial)
		{
			DeinitializeTutorial(tutorial);
			_quitButton.SetActive(value: false);
		}

		[CompilerGenerated]
		private void _003COnQuitTutorialClicked_003Eg__DismissAction_007C35_0()
		{
			_quitTutorialPopupOpen = false;
			_quitButton.SetActive(!PopupBlocksQuitButton);
		}
	}
}
