using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class LinkUseCodePopup : ToggleableInteractionPopup
	{
		[SerializeField]
		private InputField _inputField;

		private Settings _settings;

		private GameSparksServer _gameSparksServer;

		public override string AnalyticsScreenName => "link_use_code";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_settings = model.Device.Settings;
			_gameSparksServer = model.GameServer.GameSparksServer;
		}

		public void OnConfirmClicked()
		{
			Confirm();
		}

		public void OnEndEdit()
		{
			Confirm();
		}

		private void Confirm()
		{
			if (base.Interactable)
			{
				base.Interactable = false;
				string linkCode = _inputField.text;
				_settings.ToggleAuthenticationAllowed(on: true);
				_gameSparksServer.AuthenticationController.AutoAuthenticate(delegate
				{
					_gameSparksServer.GameSparksUser.GetMetaData(linkCode, OnGetMetaDataSuccess, OnGetMetaDataError);
				}, OnAuthenticationError);
			}
		}

		private void OnAuthenticationError()
		{
			base.Interactable = true;
			_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup());
		}

		private void OnGetMetaDataSuccess(string linkCode, GameSparksUser.UserMetaData metaData)
		{
			base.Interactable = true;
			_popupManager.RequestPopup(new LinkConfirmPopupRequest(linkCode, metaData));
		}

		private void OnGetMetaDataError(GameSparksException exc)
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
