using CIG.Translation;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public abstract class BuildingTutorialView<TTutorial, TBuilding> : TutorialView<TTutorial> where TTutorial : BuildingTutorial where TBuilding : Building
	{
		private const double BuildDuration = 5.0;

		private HUDShopButton _shopButton;

		private Builder _subscribedToBuilder;

		private ShopPopup _shopPopup;

		protected override bool CanShow
		{
			get
			{
				if (!base.CanShow || !base.CanShowOnIsland)
				{
					return false;
				}
				switch (_tutorial.State)
				{
				case BuildingTutorial.TutorialState.None:
					return false;
				case BuildingTutorial.TutorialState.OpenShop:
					return !_popupManagerView.IsShowingPopup;
				case BuildingTutorial.TutorialState.SelectTab:
				case BuildingTutorial.TutorialState.SelectBuilding:
					return _popupManagerView.TopPopup is ShopPopup;
				case BuildingTutorial.TutorialState.ConfirmBuilding:
					return _popupManagerView.TopPopup is BuildingPopup;
				case BuildingTutorial.TutorialState.ConfirmBuild:
					return _popupManagerView.TopPopup is BuildConfirmPopup;
				default:
					return true;
				}
			}
		}

		protected abstract ILocalizedString Text
		{
			get;
		}

		protected abstract ShopMenuTabs ShopMenuTab
		{
			get;
		}

		public void Initialize(TTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDShopButton shopButton)
		{
			_shopButton = shopButton;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
			if (HasBuiltBuildingOfType())
			{
				_tutorial.Finish();
			}
		}

		public override void Deinitialize()
		{
			if (_shopPopup != null)
			{
				_shopPopup.TabChangedEvent -= OnTabChanged;
				_shopPopup.OpenCloseTweenerStoppedPlayingEvent -= OnShopPopupTweenerStoppedPlaying;
				_shopPopup.SetScrollingEnabled(scrollingEnabled: true);
				_shopPopup = null;
			}
			if (_subscribedToBuilder != null)
			{
				_subscribedToBuilder.StartBuildingEvent -= OnStartBuilding;
				_subscribedToBuilder.StopBuildingEvent -= OnStopBuilding;
				_subscribedToBuilder.BuildingBuiltEvent -= OnBuildingBuilt;
				_subscribedToBuilder = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case BuildingTutorial.TutorialState.OpenShop:
				_tutorialDialog.Show(Text, TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, null);
				_tutorialPointer.Show(this, _shopButton.MaskTransform);
				break;
			case BuildingTutorial.TutorialState.SelectTab:
			{
				RectTransform tutorialTabTarget = _shopPopup.GetTutorialTabTarget(ShopMenuTab);
				_tutorialPointer.Show(this, tutorialTabTarget);
				break;
			}
			case BuildingTutorial.TutorialState.SelectBuilding:
			{
				RectTransform firstShopItemTransform = _shopPopup.FirstShopItemTransform;
				if (firstShopItemTransform != null)
				{
					_tutorialPointer.Show(this, firstShopItemTransform);
				}
				break;
			}
			}
		}

		protected override void OnPopupShown(Popup popup)
		{
			ShopPopup shopPopup = popup as ShopPopup;
			if (shopPopup != null)
			{
				_shopPopup = shopPopup;
				_tutorialPointer.Hide(this);
				shopPopup.OpenCloseTweenerStoppedPlayingEvent += OnShopPopupTweenerStoppedPlaying;
			}
			base.OnPopupShown(popup);
		}

		protected override void OnPopupHidden(Popup popup)
		{
			if (popup is ShopPopup && _shopPopup != null)
			{
				_shopPopup.OpenCloseTweenerStoppedPlayingEvent -= OnShopPopupTweenerStoppedPlaying;
				_shopPopup.TabChangedEvent -= OnTabChanged;
				_shopPopup.SetScrollingEnabled(scrollingEnabled: true);
				_shopPopup = null;
			}
			base.OnPopupHidden(popup);
		}

		protected override void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedToBuilder != null)
			{
				_subscribedToBuilder.StartBuildingEvent -= OnStartBuilding;
				_subscribedToBuilder.StopBuildingEvent -= OnStopBuilding;
				_subscribedToBuilder.BuildingBuiltEvent -= OnBuildingBuilt;
				_subscribedToBuilder = null;
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedToBuilder = _subscribedIsland.Builder;
				_subscribedToBuilder.StartBuildingEvent += OnStartBuilding;
				_subscribedToBuilder.StopBuildingEvent += OnStopBuilding;
				_subscribedToBuilder.BuildingBuiltEvent += OnBuildingBuilt;
			}
		}

		private bool HasBuiltBuildingOfType()
		{
			if (_subscribedIsland != null)
			{
				int i = 0;
				for (int count = _subscribedIsland.BuildingsOnIsland.Count; i < count; i++)
				{
					if (_subscribedIsland.BuildingsOnIsland[i] is TBuilding)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void OnStartBuilding(SurfaceType surfacetype)
		{
			_tutorial.StartBuilding();
		}

		private void OnStopBuilding()
		{
			_tutorial.StopBuilding(HasBuiltBuildingOfType());
		}

		private void OnBuildingBuilt(GridTile tile, bool isNewBuilding)
		{
			TBuilding val;
			if ((val = (tile as TBuilding)) != null && val.State == BuildingState.Constructing)
			{
				val.OverrideConstructionDuration(5.0);
			}
		}

		private void ShopPopupOpened()
		{
			_tutorial.ShopPopupOpened(_shopPopup.LastOpenedTab == ShopMenuTab);
			if (_shopPopup.LastOpenedTab != ShopMenuTab)
			{
				_shopPopup.SetScrollingEnabled(scrollingEnabled: false);
				_shopPopup.SetScrollPosition(0f);
				_shopPopup.TabChangedEvent += OnTabChanged;
			}
		}

		private IEnumerator ShowMaskAfterShopItemsInitialized()
		{
			RectTransform shopItemTransform = _shopPopup.FirstShopItemTransform;
			if (shopItemTransform != null)
			{
				while (shopItemTransform.anchoredPosition == Vector2.zero)
				{
					yield return null;
				}
			}
			UpdateTutorialVisibility();
		}

		private void OnShopPopupTweenerStoppedPlaying(bool closing)
		{
			if (!closing)
			{
				_shopPopup.OpenCloseTweenerStoppedPlayingEvent -= OnShopPopupTweenerStoppedPlaying;
				ShopPopupOpened();
			}
		}

		private void OnTabChanged(ShopMenuTabs tab)
		{
			if (tab == ShopMenuTab)
			{
				_shopPopup.TabChangedEvent -= OnTabChanged;
				StartCoroutine(ShowMaskAfterShopItemsInitialized());
				_tutorial.ShopPopupOpened(correctTab: true);
			}
			else
			{
				_tutorial.ShopPopupOpened(correctTab: false);
			}
		}
	}
}
