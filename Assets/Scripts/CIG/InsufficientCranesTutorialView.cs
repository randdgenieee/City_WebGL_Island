using CIG.Translation;

namespace CIG
{
	public class InsufficientCranesTutorialView : TutorialView<InsufficientCranesTutorial>
	{
		protected override bool CanShow
		{
			get
			{
				if (base.CanShow)
				{
					return base.CanShowOnIslandAndWorldMap;
				}
				return false;
			}
		}

		protected override void Show()
		{
			InsufficientCranesTutorial.TutorialState state = _tutorial.State;
			if (state == InsufficientCranesTutorial.TutorialState.InsufficientCranesText)
			{
				_tutorialDialog.Show(Localization.Key("tutorial_insufficient_cranes"), TutorialDialog.AdvisorPositionType.Right, useOverlay: true, useContinueButton: true, OnContinueClicked);
			}
		}

		private void OnContinueClicked()
		{
			_tutorial.TextShown();
		}
	}
}
