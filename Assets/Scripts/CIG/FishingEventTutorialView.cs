using CIG.Translation;

namespace CIG
{
	public class FishingEventTutorialView : TutorialView<FishingEventTutorial>
	{
		protected override bool CanShow
		{
			get
			{
				if (base.CanShow && base.CanShowOnIsland && !_popupManagerView.IsShowingPopup)
				{
					return _tutorial.FishingLocation != null;
				}
				return false;
			}
		}

		public override void Deinitialize()
		{
			if (_subscribedIsland != null)
			{
				_subscribedIsland.CameraOperator.PopDisableInputRequest(this);
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			FishingEventTutorial.TutorialState state = _tutorial.State;
			if (state == FishingEventTutorial.TutorialState.Dialog)
			{
				_tutorialPointer.ShowOnIsland(this, _tutorial.FishingLocation.transform, _tutorial.FishingLocation.Sprite);
				_tutorialDialog.Show(Localization.Key("fishing_tutorial_text"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, null);
			}
		}

		protected override void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedIsland != null)
			{
				_subscribedIsland.CameraOperator.PopDisableInputRequest(this);
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedIsland.CameraOperator.PushDisableInputRequest(this);
			}
			_tutorial.OnIslandChanged(isometricIsland);
		}
	}
}
