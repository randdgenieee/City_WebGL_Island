using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class GSRegisterPopup : ToggleableInteractionPopup
	{
		[SerializeField]
		private InputField _usernameField;

		[SerializeField]
		private InputField _passwordField;

		[SerializeField]
		private GameObject _checkMark;

		private GameSparksServer _gameSparksServer;

		private Action<bool> _closeAction;

		private bool _termsAccepted;

		public override string AnalyticsScreenName => "gs_register";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameSparksServer = model.GameServer.GameSparksServer;
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			GSRegisterPopupRequest request2 = GetRequest<GSRegisterPopupRequest>();
			_closeAction = request2.CloseAction;
			_termsAccepted = false;
			_checkMark.SetActive(value: false);
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
				_gameSparksServer.Authenticator.RegisterUserName(username, username, hashedPassword, delegate
				{
					OnRegisterSuccess(username, hashedPassword);
				}, OnRegisterError);
			}
		}

		public void OnCheckboxClicked()
		{
			_termsAccepted = !_checkMark.activeSelf;
			_checkMark.SetActive(_termsAccepted);
		}

		public void OnTermsOfServiceClicked()
		{
			Application.OpenURL("https://www.sparklingsociety.net/terms-of-service/");
		}

		private bool ValidateInput()
		{
			if (!GameSparksUsernameAuthentication.IsValidUsername(_usernameField.text))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_NAME_LENGTH_ERROR"));
				return false;
			}
			if (!GameSparksUsernameAuthentication.IsValidPassword(_passwordField.text))
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_PASSWORD_LENGTH_ERROR"));
				return false;
			}
			if (!_termsAccepted)
			{
				_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup("SSP_TOS_ACCEPT"));
				return false;
			}
			return true;
		}

		private void OnLoggedIn()
		{
			base.Interactable = true;
			OnCloseClicked();
		}

		private void OnRegisterSuccess(string username, string hashedPassword)
		{
			GameSparksUtils.SaveAccountToStorage(username, hashedPassword);
			_gameSparksServer.AuthenticationController.ToggleSocialAuthenticationAllowed(on: true, OnAuthenticationSuccess, null);
		}

		private void OnRegisterError(GameSparksException error)
		{
			string text = error.ToString();
			string errorCodeKey = (!text.Contains("USERNAME") || !text.Contains("TAKEN")) ? "social_code_error" : "SSP_DUPLICATE_USER";
			_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup(errorCodeKey));
			base.Interactable = true;
		}

		private void OnAuthenticationSuccess()
		{
			GenericPopupRequest request = new GenericPopupRequest("gs_register_success").SetDismissable(dismissable: false).SetTexts(Localization.EmptyLocalizedString, Localization.Key("SSP_ACCOUNT_CREATED")).SetGreenOkButton(OnLoggedIn);
			_popupManager.RequestPopup(request);
		}
	}
}
