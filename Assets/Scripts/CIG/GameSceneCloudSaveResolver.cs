using CIG.Translation;
using System;

namespace CIG
{
	public class GameSceneCloudSaveResolver : SceneCloudSaveResolver
	{
		private Model _model;

		private PopupManager _popupManager;

		private TutorialManagerView _tutorialManagerView;

		private SceneLoader _sceneLoader;

		public void Initialize(Model model, TutorialManagerView tutorialManagerView, SceneLoader sceneLoader)
		{
			_model = model;
			_popupManager = _model.Game.PopupManager;
			_tutorialManagerView = tutorialManagerView;
			_sceneLoader = sceneLoader;
			Initialize(model);
		}

		protected override void OnConflictAskPlayer()
		{
			_tutorialManagerView.Pointer.RegisterBlocker(this);
			_popupManager.RequestPopup(new CloudStorageConflictPopupRequest(AskPlayerPickLocal, AskPlayerPickCloud, _cloudStorage.UTCSaveTime, _cloudStorage.LastCloudGameState, OnPopupClosed));
		}

		protected override void OnConflictResultInvalidGameVersion()
		{
			_tutorialManagerView.Pointer.RegisterBlocker(this);
			GenericPopupRequest request = new GenericPopupRequest("cloudstorage_conflict_game_version").SetDismissable(dismissable: false).SetTexts(Localization.Key("conflict_gameversion_title"), Localization.Key("conflict_gameversion_body")).SetGreenOkButton(OnPopupClosed);
			_popupManager.RequestPopup(request);
		}

		protected override void OnPickCloudSuccess(Action callback)
		{
			base.OnPickCloudSuccess(callback);
			_sceneLoader.LoadScene(new GameSceneRequest(_model));
		}

		protected override void OnPickCloudError(Action callback)
		{
			base.OnPickCloudError(callback);
			GenericPopupRequest request = new GenericPopupRequest("cloudstorage_conflict_error").SetDismissable(dismissable: false).SetTexts(Localization.Key("oops_something_went_wrong"), Localization.Key("SSP_ERROR")).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void AskPlayerPickLocal(Action callback)
		{
			OnConflictResultPickLocal();
			EventTools.Fire(callback);
		}

		private void AskPlayerPickCloud(Action callback)
		{
			OnConflictResultPickCloud(callback);
		}

		private void OnPopupClosed()
		{
			_tutorialManagerView.Pointer.UnregisterBlocker(this);
		}
	}
}
