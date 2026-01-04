using CIG.Translation;

namespace CIG
{
	public class ExpansionTutorialView : TutorialView<ExpansionTutorial>
	{
		private CIGExpansions _subscribedExpansions;

		private BuyExpansionPopup _buyExpansionPopup;

		protected override bool CanShow
		{
			get
			{
				if (!base.CanShow || !base.CanShowOnIsland)
				{
					return false;
				}
				ExpansionTutorial.TutorialState state = _tutorial.State;
				if ((uint)state <= 5u || state == ExpansionTutorial.TutorialState.FinishText)
				{
					return !_popupManagerView.IsShowingPopup;
				}
				return _popupManagerView.TopPopup is BuyExpansionPopup;
			}
		}

		public void Initialize(ExpansionTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, BuyExpansionPopup buyExpansionPopup)
		{
			_buyExpansionPopup = buyExpansionPopup;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
		}

		public override void Deinitialize()
		{
			if (_subscribedExpansions != null)
			{
				_subscribedExpansions.ExpansionUnlockedEvent -= OnExpansionUnlocked;
				_subscribedExpansions = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case ExpansionTutorial.TutorialState.IntroText1:
				_tutorialDialog.Show(Localization.Key("tutorial.expansion_1"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case ExpansionTutorial.TutorialState.IntroText2:
				_tutorialDialog.Show(Localization.Key("tutorial.expansion_2"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case ExpansionTutorial.TutorialState.IntroText3:
				_tutorialDialog.Show(Localization.Key("tutorial.show_you"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case ExpansionTutorial.TutorialState.IntroText4:
				_tutorialDialog.Show(Localization.Key("tutorial.expansion_3"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case ExpansionTutorial.TutorialState.OpenExpansion:
				_tutorialPointer.ShowOnIsland(this, _tutorial.ExpansionBlock.BuySign.transform, _tutorial.ExpansionBlock.BuySign.SpriteRenderer.sprite);
				IsometricIsland.Current.CameraOperator.PushDisableInputRequest(this);
				IsometricIsland.Current.CameraOperator.ScrollTo(_tutorial.ExpansionBlock.BuySign.gameObject);
				break;
			case ExpansionTutorial.TutorialState.BuyExpansion:
				_tutorialPointer.Show(this, _buyExpansionPopup.BuyWithGoldMaskTransform);
				break;
			case ExpansionTutorial.TutorialState.FinishText:
				_tutorialDialog.Show(Localization.Key("tutorial.well_done"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
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
			if (_subscribedExpansions != null)
			{
				_subscribedExpansions.ExpansionUnlockedEvent -= OnExpansionUnlocked;
				_subscribedExpansions = null;
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedExpansions = _subscribedIsland.Expansions;
				_subscribedExpansions.ExpansionUnlockedEvent += OnExpansionUnlocked;
			}
			_tutorial.OnIslandChanged(isometricIsland);
		}

		private void OnContinueClicked()
		{
			_tutorial.TextDismissed();
		}

		private void OnExpansionUnlocked(ExpansionBlock expansionBlock)
		{
			_tutorial.ExpansionUnlocked();
		}
	}
}
