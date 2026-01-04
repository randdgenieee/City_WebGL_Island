using CIG.Translation;

namespace CIG
{
	public class WorldMapStartTutorialView : TutorialView<WorldMapStartTutorial>
	{
		protected override bool CanShow
		{
			get
			{
				if (base.CanShow && !_popupManagerView.IsShowingPopup)
				{
					return base.CanShowOnWorldMap;
				}
				return false;
			}
		}

		protected override void Show()
		{
			switch (_tutorial.State)
			{
			case WorldMapStartTutorial.TutorialState.UnlockNewIslandsText:
				_tutorialDialog.Show(Localization.Key("tutorial_unlock_new_islands_here"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case WorldMapStartTutorial.TutorialState.MoveAirshipText:
				_tutorialDialog.Show(Localization.Key("tutorial_move_airship"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			}
		}

		private void OnContinueClicked()
		{
			_tutorial.TextShown();
		}
	}
}
