using CIG.Translation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class TreasureChestTutorialView : TutorialView<TreasureChestTutorial>
	{
		private ShopPopup _shopPopup;

		private HUDShopButton _shopButton;

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
				case TreasureChestTutorial.TutorialState.None:
					return false;
				case TreasureChestTutorial.TutorialState.OpenShopPopup:
					return !_popupManagerView.IsShowingPopup;
				case TreasureChestTutorial.TutorialState.OpenChestTab:
				case TreasureChestTutorial.TutorialState.OpenChest:
					return _popupManagerView.TopPopup is ShopPopup;
				default:
					return true;
				}
			}
		}

		public void Initialize(TreasureChestTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, HUDShopButton shopButton)
		{
			_shopButton = shopButton;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
		}

		public override void Deinitialize()
		{
			ReleaseShopPopup();
			base.Deinitialize();
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case TreasureChestTutorial.TutorialState.OpenShopPopup:
				_tutorialDialog.Show(Localization.Key("tutorial.treasure_chest_openable"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, null);
				_tutorialPointer.Show(this, _shopButton.MaskTransform);
				break;
			case TreasureChestTutorial.TutorialState.OpenChestTab:
				_tutorialDialog.Show(Localization.Key("tutorial.treasure_chest_openable"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, null);
				_tutorialPointer.Show(this, _shopPopup.GetTutorialTabTarget(ShopMenuTabs.Chests));
				break;
			case TreasureChestTutorial.TutorialState.OpenChest:
				_tutorialDialog.Show(Localization.Key("tutorial.treasure_chest_openable"), TutorialDialog.AdvisorPositionType.Left, useOverlay: false, useContinueButton: true, OnContinueClicked);
				_tutorialPointer.Show(this, GetOpenableChestRectTransform());
				break;
			}
		}

		protected override void OnPopupShown(Popup popup)
		{
			ShopPopup shopPopup;
			if ((object)(shopPopup = (popup as ShopPopup)) != null)
			{
				_shopPopup = shopPopup;
				_shopPopup.OpenCloseTweenerStoppedPlayingEvent += OnShopPopupTweenerStoppedPlaying;
			}
			else
			{
				base.OnPopupShown(popup);
			}
		}

		protected override void OnPopupHidden(Popup popup)
		{
			if (popup is ShopPopup)
			{
				ReleaseShopPopup();
			}
			else
			{
				base.OnPopupHidden(popup);
			}
		}

		private void OnContinueClicked()
		{
			_tutorial.Finish();
		}

		private RectTransform GetOpenableChestRectTransform()
		{
			TreasureChestType? openableChestType = _tutorial.GetOpenableChestType();
			if (openableChestType.HasValue)
			{
				List<ShopItem> shopItemViews = _shopPopup.GetShopItemViews(ShopType.Shop_chests);
				int i = 0;
				for (int count = shopItemViews.Count; i < count; i++)
				{
					ChestShopItem chestShopItem = shopItemViews[i] as ChestShopItem;
					if (chestShopItem != null && chestShopItem.TreasureChestType == openableChestType.Value)
					{
						return (RectTransform)chestShopItem.transform;
					}
				}
			}
			UnityEngine.Debug.LogError("No openable chest could be found.");
			return null;
		}

		private void ReleaseShopPopup()
		{
			if (_shopPopup != null)
			{
				_shopPopup.OpenCloseTweenerStoppedPlayingEvent -= OnShopPopupTweenerStoppedPlaying;
				_shopPopup.TabChangedEvent -= OnTabChanged;
				_shopPopup.SetScrollingEnabled(scrollingEnabled: true);
				_shopPopup = null;
			}
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
			if (tab == ShopMenuTabs.Chests)
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

		private void ShopPopupOpened()
		{
			_tutorial.ShopPopupOpened(_shopPopup.LastOpenedTab == ShopMenuTabs.Chests);
			if (_shopPopup.LastOpenedTab != ShopMenuTabs.Chests)
			{
				_shopPopup.SetScrollingEnabled(scrollingEnabled: false);
				_shopPopup.TabChangedEvent += OnTabChanged;
			}
		}

		private IEnumerator ShowMaskAfterShopItemsInitialized()
		{
			List<ShopItem> shopItems = _shopPopup.GetShopItemViews(ShopType.Shop_chests);
			int i = 0;
			for (int count = shopItems.Count; i < count; i++)
			{
				RectTransform shopItemTransform = shopItems[i].transform as RectTransform;
				if (shopItemTransform != null)
				{
					while (shopItemTransform.anchoredPosition == Vector2.zero)
					{
						yield return null;
					}
				}
			}
			UpdateTutorialVisibility();
		}
	}
}
