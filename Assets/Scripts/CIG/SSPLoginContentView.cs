using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class SSPLoginContentView : SSPMenuContentView
	{
		[SerializeField]
		private LocalizedText _titleText;

		[SerializeField]
		private LocalizedText _bodyText;

		[SerializeField]
		private LocalizedText _playerNameText;

		[SerializeField]
		private LocalizedText _loginText;

		[SerializeField]
		private GameObject _playerNameRoot;

		[SerializeField]
		private SocialLoginButton _socialLoginButton;

		private Settings _settings;

		private GameSparksServer _gameSparksServer;

		public override ILocalizedString HeaderText => Localization.Key("sparksoc.login");

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.Login;

		public override void Initialize(SSPMenuPopup popup, Model model)
		{
			base.Initialize(popup, model);
			_settings = model.Device.Settings;
			_gameSparksServer = model.GameServer.GameSparksServer;
			_socialLoginButton.Initialize(model.Game.PopupManager, _settings, _gameSparksServer);
			_gameSparksServer.Authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
			CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
			gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Combine(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
			_settings.SettingChangedEvent += OnSettingChanged;
			_titleText.LocalizedString = Localization.Format(Localization.Key("login_text"), Localization.Literal(CIGGameConstants.SocialServiceName));
		}

		public override void Deinitialize()
		{
			if (_gameSparksServer != null)
			{
				_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
				CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
				gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
				_gameSparksServer = null;
			}
			if (_settings != null)
			{
				_settings.SettingChangedEvent -= OnSettingChanged;
			}
			base.Deinitialize();
		}

		protected override void Open()
		{
			base.Open();
			OnAuthenticationChanged(_gameSparksServer.Authenticator.CurrentAuthentication, null);
		}

		private void UpdateContent()
		{
			if (_settings.SocialAuthenticationAllowed)
			{
				_loginText.LocalizedString = Localization.Key("log_out");
				if (_gameSparksServer.AuthenticationController.HasConnectionFailure)
				{
					_bodyText.LocalizedString = Localization.Key("sparksoc.general_error");
					_playerNameRoot.SetActive(value: false);
				}
				else
				{
					_bodyText.LocalizedString = Localization.Key("logged_in_as");
					_playerNameRoot.SetActive(value: true);
					_playerNameText.LocalizedString = Localization.Literal(_gameSparksServer.Authenticator.CurrentAuthentication.DisplayName);
				}
			}
			else
			{
				_bodyText.LocalizedString = Localization.Key("logged_out");
				_loginText.LocalizedString = Localization.Key("social_login");
				_playerNameRoot.SetActive(value: false);
			}
		}

		private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			UpdateContent();
		}

		private void OnGameSparksAvailable(bool available)
		{
			UpdateContent();
		}

		private void OnSettingChanged()
		{
			UpdateContent();
		}
	}
}
