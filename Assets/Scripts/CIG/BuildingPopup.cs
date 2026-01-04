using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class BuildingPopup : Popup
	{
		[SerializeField]
		private LocalizedText _titleLabel;

		[SerializeField]
		private LocalizedText _subtitleLabel;

		[SerializeField]
		private BuildingPopupIconContainer _iconContainer;

		[SerializeField]
		private BuildingPopupStatsContainer _statsContainer;

		[SerializeField]
		private BuildingPopupExchangingContainer _exchangingContainer;

		[SerializeField]
		private BuildingPopupExchangeContainer _exchangeContainer;

		[SerializeField]
		private Button _moveButton;

		[SerializeField]
		private Button _warehouseButton;

		[SerializeField]
		private Button _demolishButton;

		[SerializeField]
		private InteractableButton _waitButton;

		[SerializeField]
		private InteractableButton _instantButton;

		[SerializeField]
		private GameObject _landmarkGlow;

		[SerializeField]
		private RectTransform _warehouseButtonMaskTransform;

		private BuildingWarehouseManager _warehouseManager;

		private CurrencyConversionManager _currencyConversionManager;

		private VideoAds2Manager _videoAds2Manager;

		private IslandsManager _islandsManager;

		private WorldMap _worldMap;

		private GameState _gameState;

		private GameStats _gameStats;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private Multipliers _multipliers;

		private BuildingProperties _buildingProperties;

		private CIGCommercialBuilding _commercialBuilding;

		private bool _isBusy;

		public override string AnalyticsScreenName => "building";

		public CIGBuilding Building
		{
			get;
			private set;
		}

		public BuildingPopupContent Content
		{
			get;
			private set;
		}

		public RectTransform WarehouseButtonMaskTransform => _warehouseButtonMaskTransform;

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_warehouseManager = model.Game.BuildingWarehouseManager;
			_currencyConversionManager = model.Game.CurrencyConversionManager;
			_videoAds2Manager = model.Game.VideoAds2Manager;
			_islandsManager = model.Game.IslandsManager;
			_worldMap = model.Game.WorldMap;
			_gameState = model.Game.GameState;
			_gameStats = model.Game.GameStats;
			_buildingWarehouseManager = model.Game.BuildingWarehouseManager;
			_multipliers = model.GameServer.WebService.Multipliers;
			_exchangingContainer.Initialize(_videoAds2Manager, model.Game.Timing);
			_statsContainer.Initialize(model.Game.Timing, _gameState, _gameStats, _buildingWarehouseManager, _multipliers);
			_exchangeContainer.Initialize(model.Game.GameState);
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			BuildingPopupRequest request2 = GetRequest<BuildingPopupRequest>();
			Building = request2.Building;
			_buildingProperties = request2.BuildingProperties;
			Content = request2.Content;
			_commercialBuilding = (Building as CIGCommercialBuilding);
			if (_commercialBuilding != null)
			{
				_commercialBuilding.CurrencyConversionStateChangedEvent += OnCurrencyConversionStateChanged;
			}
			if (Building == null)
			{
				_iconContainer.Show(_buildingProperties);
				_statsContainer.Show(_buildingProperties);
			}
			else
			{
				_iconContainer.Show(Building);
				_statsContainer.Show(Building);
			}
			_landmarkGlow.gameObject.SetActive(_buildingProperties is LandmarkBuildingProperties);
			_waitButton.interactable = !_worldMap.IsVisible;
			_instantButton.interactable = !_worldMap.IsVisible;
			UpdateContent();
		}

		public void OnMoveButtonClicked()
		{
			if (!_isBusy)
			{
				_isBusy = true;
				OnCloseClicked();
				_popupManager.RequestPopup(new BuildConfirmMovePopupRequest(Building, moveCameraToTarget: true));
			}
		}

		public void OnWarehouseButtonClicked()
		{
			if (!_isBusy)
			{
				_isBusy = true;
				OnCloseClicked();
				if (Building.DemolishImmediately(moveToWarehouse: true))
				{
					_warehouseManager.SaveBuilding(_buildingProperties, Building.CurrentLevel, Building.WasBuiltWithCash, newBuilding: false, WarehouseSource.OnePointFive);
				}
			}
		}

		public void OnDemolishButtonClicked()
		{
			if (!_isBusy)
			{
				_isBusy = true;
				CIGBuilding building = Building;
				GenericPopupRequest request = new GenericPopupRequest("building_demolish_confirm").SetTexts(_buildingProperties.LocalizedName, Localization.Key("destroy_building_confirm"), Localization.Key("demolish")).SetDismissable(dismissable: true, delegate
				{
					_isBusy = false;
				}).SetGreenOkButton(delegate
				{
					OnCloseClicked();
					building.StartDemolishing();
				})
					.SetRedCancelButton(delegate
					{
						_isBusy = false;
					})
					.SetIcon(_buildingProperties, SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.DemolishIcon));
				_popupManager.RequestPopup(request);
			}
		}

		public void OnCashButtonClicked()
		{
			if (!_isBusy)
			{
				_isBusy = true;
				BuyOrUpgrade(instant: false);
			}
		}

		public void OnInstantButtonClicked()
		{
			if (!_isBusy)
			{
				_isBusy = true;
				BuyOrUpgrade(instant: true);
			}
		}

		public void OnSpeedupCurrencyConversionClicked()
		{
			if (!_isBusy && _commercialBuilding != null)
			{
				_isBusy = true;
				CIGCommercialBuilding commercialBuilding = _commercialBuilding;
				_videoAds2Manager.ShowAd(delegate(bool success, bool clicked)
				{
					if (success)
					{
						commercialBuilding.CurrencyConversionProcess?.FreeSpeedup();
					}
					_isBusy = false;
				}, VideoSource.CurrencyConversionSpeedup);
			}
		}

		protected override void Closed()
		{
			_statsContainer.Hide();
			_exchangingContainer.Hide();
			_exchangeContainer.Hide();
			if (_commercialBuilding != null)
			{
				_commercialBuilding.CurrencyConversionStateChangedEvent -= OnCurrencyConversionStateChanged;
				_commercialBuilding = null;
			}
			Building = null;
			_isBusy = false;
			base.Closed();
		}

		private void BuyOrUpgrade(bool instant)
		{
			if (Content == BuildingPopupContent.Preview)
			{
				BuyBuilding(instant);
			}
			else
			{
				UpgradeBuilding(instant);
			}
		}

		private void BuyBuilding(bool instant)
		{
			if (IsometricIsland.Current.IsometricGrid.ElementTypeAvailable(_buildingProperties.SurfaceType))
			{
				Currency constructionCost = _buildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager);
				_popupManager.RequestPopup(new BuildConfirmBuyPopupRequest(_buildingProperties, instant ? _buildingProperties.GetInstantConstructionCost(_gameState, _gameStats, _buildingWarehouseManager, _multipliers) : constructionCost, constructionCost.IsMatchingName("Cash"), instant ? BuildFinishType.Instant : BuildFinishType.Normal));
				_popupManager.CloseAllOpenPopups(instant: false);
			}
			else
			{
				_popupManager.RequestPopup(GenericPopupRequest.UnavailableSurfaceTypePopupRequest(_popupManager, _islandsManager, _buildingProperties));
				_isBusy = false;
			}
		}

		private void UpgradeBuilding(bool instant)
		{
			if (Building.CanUpgrade)
			{
				if (instant)
				{
					Building.UpgradeImmediately();
				}
				else
				{
					Building.StartUpgrade();
				}
				OnCloseClicked();
				return;
			}
			ILocalizedString localizedString = Building.ReasonWhyCantUpgrade();
			if (localizedString != null && !string.IsNullOrEmpty(localizedString.Translate()))
			{
				GenericPopupRequest request = new GenericPopupRequest("building_cant_upgrade").SetTexts(_buildingProperties.LocalizedName, localizedString).SetGreenOkButton();
				_popupManager.RequestPopup(request);
				_isBusy = false;
			}
		}

		private void UpdateContent()
		{
			switch (Content)
			{
			case BuildingPopupContent.Activate:
				SetActivateContent();
				break;
			case BuildingPopupContent.Upgrade:
				if (_commercialBuilding != null)
				{
					if (_commercialBuilding.CurrencyConversionProcess == null)
					{
						SetUpgradeContent();
					}
					else
					{
						SetExchangingContent(_commercialBuilding);
					}
				}
				else
				{
					SetUpgradeContent();
				}
				SetExchangeContent(_commercialBuilding);
				break;
			case BuildingPopupContent.Preview:
				SetPreviewContent();
				break;
			case BuildingPopupContent.LandmarkPreview:
				SetLandmarkPreviewContent();
				break;
			}
			SetTitle(_buildingProperties.LocalizedName);
		}

		private void SetPreviewContent()
		{
			SetSubtitle((_buildingProperties.SurfaceType == SurfaceType.AnyTypeOfLand) ? null : Localization.Format(Localization.Key("must_be_built_on"), _buildingProperties.SurfaceType.ToLocalizedString()));
			SetBuildingManagementButtonsVisible(visible: false);
			_exchangeContainer.Hide();
			_exchangingContainer.Hide();
			_statsContainer.ShowPreviewContent(_buildingProperties);
		}

		private void SetLandmarkPreviewContent()
		{
			SetSubtitle((_buildingProperties.SurfaceType == SurfaceType.AnyTypeOfLand) ? null : Localization.Format(Localization.Key("must_be_built_on"), _buildingProperties.SurfaceType.ToLocalizedString()));
			SetBuildingManagementButtonsVisible(visible: false);
			_exchangeContainer.Hide();
			_exchangingContainer.Hide();
			_statsContainer.ShowLandmarkPreviewContent(_buildingProperties);
		}

		private void SetActivateContent()
		{
			SetSubtitle(Localization.Key("building_activate"));
			SetBuildingManagementButtonsVisible(visible: false);
			_exchangeContainer.Hide();
			_exchangingContainer.Hide();
			_statsContainer.ShowActivateContent(Building);
		}

		private void SetUpgradeContent()
		{
			bool flag = _buildingProperties.MaximumLevel == Building.CurrentLevel;
			if (flag)
			{
				SetSubtitle(Localization.Format(Localization.Key("level_x"), Localization.Integer(Building.CurrentLevel)));
			}
			else
			{
				int num = Building.CurrentLevel - (_buildingProperties.Activatable ? 1 : 0);
				SetSubtitle(Localization.Format(Localization.Key("upgrade_to_level_x"), Localization.Integer(num + 1)));
			}
			SetBuildingManagementButtonsVisible(visible: true);
			SetMoveButtonInteractable(_buildingProperties.Movable);
			SetWarehouseButtonInteractable(Building.CanMoveToWarehouse && _warehouseManager.VacantUnlockedSlots > 0);
			SetDemolishButtonInteractable(_buildingProperties.Destructible);
			_exchangingContainer.Hide();
			_statsContainer.ShowUpgradeContent(Building, flag);
		}

		private void SetExchangingContent(CIGCommercialBuilding commercialBuilding)
		{
			SetSubtitle(Localization.Key("exchanging"));
			SetBuildingManagementButtonsVisible(visible: true);
			SetMoveButtonInteractable(commercialBuilding.BuildingProperties.Movable);
			SetWarehouseButtonInteractable(interactable: false);
			SetDemolishButtonInteractable(interactable: false);
			_statsContainer.Hide();
			_exchangingContainer.Show(commercialBuilding);
		}

		private void SetExchangeContent(CIGCommercialBuilding commercialBuilding)
		{
			if (_currencyConversionManager.FeatureEnabled && commercialBuilding != null && commercialBuilding.CurrencyConversionProcess == null)
			{
				_exchangeContainer.Show(commercialBuilding, _currencyConversionManager.ConversionProperties, OnCurrencyConversionButtonClicked);
			}
			else
			{
				_exchangeContainer.Hide();
			}
		}

		private void SetMoveButtonInteractable(bool interactable)
		{
			_moveButton.interactable = interactable;
		}

		private void SetWarehouseButtonInteractable(bool interactable)
		{
			_warehouseButton.interactable = interactable;
		}

		private void SetDemolishButtonInteractable(bool interactable)
		{
			_demolishButton.interactable = interactable;
		}

		private void SetBuildingManagementButtonsVisible(bool visible)
		{
			_moveButton.gameObject.SetActive(visible);
			_warehouseButton.gameObject.SetActive(visible);
			_demolishButton.gameObject.SetActive(visible);
		}

		private void SetSubtitle(ILocalizedString text)
		{
			_subtitleLabel.gameObject.SetActive(!Localization.IsNullOrEmpty(text));
			_subtitleLabel.LocalizedString = text;
		}

		private void SetTitle(ILocalizedString text)
		{
			_titleLabel.LocalizedString = text;
		}

		private void OnCurrencyConversionButtonClicked(CIGCommercialBuilding commercialBuilding, CurrencyConversionProperties properties)
		{
			commercialBuilding.StartCurrencyConversion(properties);
		}

		private void OnCurrencyConversionStateChanged(bool active)
		{
			if (active)
			{
				UpdateContent();
			}
			else
			{
				OnCloseClicked();
			}
		}
	}
}
