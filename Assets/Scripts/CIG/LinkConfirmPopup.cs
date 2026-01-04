using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class LinkConfirmPopup : ToggleableInteractionPopup
	{
		[SerializeField]
		private LocalizedText _displayNameLabel;

		[SerializeField]
		private LocalizedText _levelLabel;

		private Model _model;

		private GameSparksServer _gameSparksServer;

		private SceneLoader _sceneLoader;

		private string _linkCode;

		public override string AnalyticsScreenName => "link_confirm";

		public void Initialize(SceneLoader sceneLoader)
		{
			_sceneLoader = sceneLoader;
		}

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_model = model;
			_gameSparksServer = _model.GameServer.GameSparksServer;
		}

		protected override void OnDestroy()
		{
			if (_gameSparksServer != null)
			{
				_gameSparksServer.CloudStorage.PopPushBlockingObject(this);
				_gameSparksServer = null;
			}
			_model = null;
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			LinkConfirmPopupRequest linkConfirmPopupRequest = (LinkConfirmPopupRequest)request;
			_linkCode = linkConfirmPopupRequest.LinkCode;
			_displayNameLabel.LocalizedString = Localization.Literal(linkConfirmPopupRequest.MetaData.DisplayName);
			_levelLabel.LocalizedString = Localization.Integer(linkConfirmPopupRequest.MetaData.PlayerLevel);
		}

		public void OnConfirm()
		{
			if (base.Interactable)
			{
				base.Interactable = false;
				_gameSparksServer.CloudStorage.PushPushBlockingObject(this);
				_gameSparksServer.GameSparksUser.LinkPlayer(_linkCode, OnLinkSuccess, OnLinkError);
			}
		}

		public void OnCancel()
		{
			OnCloseClicked();
		}

		private void ReloadGameScene()
		{
			_sceneLoader.LoadScene(new GameSceneRequest(_model));
		}

		private void OnLinkSuccess()
		{
			_gameSparksServer.CloudStorage.ForcePull(OnForcePullSuccess, OnError);
		}

		private void OnForcePullSuccess()
		{
			GenericPopupRequest request = new GenericPopupRequest("link_device_success").SetDismissable(dismissable: false).SetTexts(Localization.Key("link_device"), Localization.Key("link_account_success")).SetGreenOkButton(ReloadGameScene);
			_popupManager.RequestPopup(request);
		}

		private void OnLinkError(GameSparksException exc)
		{
			_gameSparksServer.CloudStorage.PopPushBlockingObject(this);
			OnError(exc);
		}

		private void OnError(GameSparksException exc)
		{
			base.Interactable = true;
			GameSparksUtils.LogGameSparksError(exc);
			string errorCodeKey;
			switch (exc.Error)
			{
			case GSError.AlreadyLinkedToSameUser:
				errorCodeKey = "already_linked";
				break;
			case GSError.InvalidCode:
				errorCodeKey = "social_code_unknown";
				break;
			default:
				errorCodeKey = "social_code_error";
				break;
			}
			_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup(errorCodeKey));
		}
	}
}
