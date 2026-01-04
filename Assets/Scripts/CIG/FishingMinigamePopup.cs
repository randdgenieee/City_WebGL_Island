using UnityEngine;

namespace CIG
{
	public class FishingMinigamePopup : Popup
	{
		[SerializeField]
		private GameObject _headerRoot;

		[SerializeField]
		private GameObject _background;

		[SerializeField]
		private GameObject _startContent;

		[SerializeField]
		private FishingMinigamePopupPlayingContent _playingContent;

		[SerializeField]
		private FishingMinigamePopupEndContent _endContent;

		private OverlayManager _overlayManager;

		private FishingEvent _fishingEvent;

		private FishingMinigame _fishingMinigame;

		public override string AnalyticsScreenName => "fishing_minigame";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_fishingEvent = model.Game.FishingEvent;
			_playingContent.MinigameFinishedEvent += OnMinigameFinished;
		}

		protected override void OnDestroy()
		{
			if (_playingContent != null)
			{
				_playingContent.MinigameFinishedEvent -= OnMinigameFinished;
			}
			if (IsometricIsland.Current != null)
			{
				IsometricIsland.Current.CameraOperator.PopDisableInputRequest(this);
			}
			if (_overlayManager != null)
			{
				_overlayManager.EnableInteractionRequest(this);
				_overlayManager = null;
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			FishingMinigamePopupRequest request2 = GetRequest<FishingMinigamePopupRequest>();
			_fishingMinigame = request2.FishingMinigame;
			SetContent(startEnabled: true, reelEnabled: false, endEnabled: false);
			IsometricIsland current = IsometricIsland.Current;
			current.CameraOperator.PushDisableInputRequest(this);
			_overlayManager = current.OverlayManager;
			_overlayManager.DisableInteractionRequest(this);
		}

		public void OnStartClicked()
		{
			SetContent(startEnabled: false, reelEnabled: true, endEnabled: false);
			_playingContent.Initialize(_fishingMinigame);
		}

		protected override void Closed()
		{
			_fishingEvent.StopMinigame();
			_endContent.Deinitialize();
			_playingContent.Deinitialize();
			_overlayManager.EnableInteractionRequest(this);
			IsometricIsland.Current.CameraOperator.PopDisableInputRequest(this);
			base.Closed();
		}

		private void SetContent(bool startEnabled, bool reelEnabled, bool endEnabled)
		{
			_startContent.SetActive(startEnabled);
			_playingContent.SetActive(reelEnabled);
			_endContent.SetActive(endEnabled);
			_headerRoot.SetActive(startEnabled | endEnabled);
			_background.SetActive(startEnabled | endEnabled);
		}

		private void OnMinigameFinished()
		{
			SetContent(startEnabled: false, reelEnabled: false, endEnabled: true);
			_endContent.Initialize(_fishingMinigame.Score, _fishingEvent.FishingQuest.Quest.Progress, _fishingEvent.FishingQuest.Quest.TargetAmount);
		}
	}
}
