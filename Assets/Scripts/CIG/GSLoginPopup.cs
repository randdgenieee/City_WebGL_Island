using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class GSLoginPopup : ToggleableInteractionPopup
	{
		[SerializeField]
		private LocalizedText _bodyText;

		[SerializeField]
		private InputField _usernameField;

		[SerializeField]
		private InputField _passwordField;

		private GameSparksServer _gameSparksServer;

		private Action<bool> _closeAction;

		public override string AnalyticsScreenName => "gs_login";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameSparksServer = model.GameServer.GameSparksServer;
			_bodyText.LocalizedString = Localization.Format(Localization.Key("login_text"), Localization.Literal(CIGGameConstants.SocialServiceName));
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			GSLoginPopupRequest request2 = GetRequest<GSLoginPopupRequest>();
			_closeAction = request2.CloseAction;
		}

		public override void Close(bool instant)
		{
			EventTools.Fire(_closeAction, _gameSparksServer.Authenticator.IsAuthenticatedWith<GameSparksUsernameAuthentication>());
			base.Close(instant);
		}

		public void OnConfirmButtonClicked()
		{
			if (base.Interactable)
			{
				base.Interactable = false;
				if (!ValidateInput())
				{
					base.Interactable = true;
					return;
				}
				string username = _usernameField.text;
				string text = _passwordField.text;
				string hashedPassword = text.ToHashedString();
				_gameSparksServer.Authenticator.AuthenticateUserName(username, hashedPassword, delegate
				{
					OnAuthenticationSuccess(username, hashedPassword);
				}, OnAuthenticationError);
			}
		}

		public void OnRegisterClicked()
		{
			_popupManager.RequestPopup(new GSRegisterPopupRequest(OnRegisterPopupClosed));
		}

		private bool ValidateInput()
		{
			if (string.IsNullOrEmpty(_usernameField.text))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_USERNAME_EMPTY"));
				return false;
			}
			if (string.IsNullOrEmpty(_passwordField.text))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_PASSWORD_EMPTY"));
				return false;
			}
			return true;
		}

		private void OnLoggedIn()
		{
			base.Interactable = true;
			OnCloseClicked();
		}

		private void OnRegisterPopupClosed(bool registered)
		{
			if (registered)
			{
				OnCloseClicked();
			}
		}

		private void OnAuthenticationError(GameSparksException exc)
		{
			GameSparksUtils.LogGameSparksError(exc);
			string text = exc.ToString();
			if (text.Contains("DETAILS") && text.Contains("UNRECOGNISED"))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_INVALID_USERNAME_OR_PASSWORD"));
				base.Interactable = true;
			}
			else if (text.Contains("DETAILS") && text.Contains("LOCKED"))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("account_locked_temporarily"));
				base.Interactable = true;
			}
		}

		private void OnAuthenticationSuccess(string username, string hashedPassword)
		{
			GameSparksUtils.SaveAccountToStorage(username, hashedPassword);
			GenericPopupRequest request = new GenericPopupRequest("gs_login_success").SetDismissable(dismissable: false).SetTexts(Localization.EmptyLocalizedString, Localization.Format(Localization.Key("login_user_success"), Localization.Literal(username))).SetGreenOkButton(OnLoggedIn);
			_gameSparksServer.AuthenticationController.ToggleSocialAuthenticationAllowed(on: true, delegate
			{
				_popupManager.RequestPopup(request);
			}, null);
		}
	}
}
