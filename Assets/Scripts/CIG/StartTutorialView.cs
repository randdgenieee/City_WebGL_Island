using CIG.Translation;

namespace CIG
{
	public class StartTutorialView : TutorialView<StartTutorial>
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
			case StartTutorial.TutorialState.WelcomeText:
				_tutorialDialog.Show(Localization.Key("tutorial.welcome_to_island"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case StartTutorial.TutorialState.BuildCityText:
				_tutorialDialog.Show(Localization.Key("tutorial.build_city_and_flourish"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			case StartTutorial.TutorialState.LetsGoText:
				_tutorialDialog.Show(Localization.Key("tutorial_lets_go"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
				break;
			}
		}

		private void OnContinueClicked()
		{
			_tutorial.TextShown();
		}
	}
}
