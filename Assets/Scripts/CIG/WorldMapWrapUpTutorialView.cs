using CIG.Translation;

namespace CIG
{
	public class WorldMapWrapUpTutorialView : TutorialView<WorldMapWrapUpTutorial>
	{
		protected override bool CanShow
		{
			get
			{
				if (base.CanShow && !_popupManagerView.IsShowingPopup)
				{
					return base.CanShowOnIsland;
				}
				return false;
			}
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case WorldMapWrapUpTutorial.TutorialState.BuildMoreText:
				_tutorialDialog.Show(Localization.Key("tutorial_build_more"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case WorldMapWrapUpTutorial.TutorialState.UnlockMoreIslandsText:
				_tutorialDialog.Show(Localization.Key("tutorial_unlock_more_islands"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			}
		}

		private void OnContinueClicked()
		{
			_tutorial.TextShown();
		}
	}
}
