using CIG.Translation;

namespace CIG
{
	public class CurrencyConversionTutorialView : TutorialView<CurrencyConversionTutorial>
	{
		private IsometricGrid _subscribedGrid;

		protected override bool CanShow
		{
			get
			{
				if (!base.CanShow || !base.CanShowOnIsland)
				{
					return false;
				}
				CurrencyConversionTutorial.TutorialState state = _tutorial.State;
				if ((uint)state <= 2u)
				{
					return !_popupManagerView.IsShowingPopup;
				}
				BuildingPopup buildingPopup = _popupManagerView.TopPopup as BuildingPopup;
				if (buildingPopup != null)
				{
					return buildingPopup.Building is CIGCommercialBuilding;
				}
				return false;
			}
		}

		public override void Deinitialize()
		{
			if (_subscribedGrid != null)
			{
				_subscribedGrid.GridTileRemovedEvent -= OnTileRemoved;
				_subscribedGrid = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case CurrencyConversionTutorial.TutorialState.IntroText:
				_tutorialDialog.Show(Localization.Key("tutorial_currency_conversion_unlocked"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case CurrencyConversionTutorial.TutorialState.OpenBuilding:
				Builder.TilesHidden = false;
				_tutorialDialog.Show(Localization.Key("tutorial_currency_conversion_click_building"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, OnContinueClicked);
				_tutorialPointer.ShowOnIsland(this, _tutorial.CommercialBuilding.transform, _tutorial.CommercialBuilding.SpriteRenderer.sprite);
				IsometricIsland.Current.CameraOperator.PushDisableInputRequest(this);
				IsometricIsland.Current.CameraOperator.ScrollTo(_tutorial.CommercialBuilding.gameObject);
				break;
			case CurrencyConversionTutorial.TutorialState.ShowButtons:
				_tutorialDialog.Show(Localization.Key("tutorial_currency_conversion_popup_explanation"), TutorialDialog.AdvisorPositionType.Left, useOverlay: false, useContinueButton: true, OnContinueClicked);
				break;
			}
		}

		protected override void Hide()
		{
			base.Hide();
			if (IsometricIsland.Current != null)
			{
				IsometricIsland.Current.CameraOperator.PopDisableInputRequest(this);
			}
		}

		protected override void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedGrid != null)
			{
				_subscribedGrid.GridTileRemovedEvent -= OnTileRemoved;
				_subscribedGrid = null;
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedGrid = _subscribedIsland.IsometricGrid;
				_subscribedGrid.GridTileRemovedEvent += OnTileRemoved;
			}
			_tutorial.UpdateBuilding();
		}

		private void OnContinueClicked()
		{
			_tutorial.TextDismissed();
		}

		private void OnTileRemoved(GridTile tile)
		{
			_tutorial.TileRemoved();
		}
	}
}
