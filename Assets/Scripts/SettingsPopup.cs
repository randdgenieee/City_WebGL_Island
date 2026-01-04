using CIG;
using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : Popup
{
	[SerializeField]
	private LocalizedText _headerText;

	[Header("Settings Tab")]
	[SerializeField]
	private GameObject _settingsTabRoot;

	[SerializeField]
	private ToggleButton _musicButton;

	[SerializeField]
	private ToggleButton _soundButton;

	[SerializeField]
	private ToggleButton _notificationButton;

	[SerializeField]
	private LocalizedText _socialBodyText;

	[SerializeField]
	private SocialLoginButton _socialLoginButton;

	[SerializeField]
	private Button _languageButton;

	[SerializeField]
	private LocalizedText _languageButtonText;

	[SerializeField]
	private LocalizedText _gameVersionText;

	[SerializeField]
	private LocalizedText _playerIdLabel;

	[Header("Language Tab")]
	[SerializeField]
	private GameObject _languageTabRoot;

	[SerializeField]
	private LanguageItem _languageItemPrefab;

	[SerializeField]
	private RectTransform _languageItemContentContainer;

	private Model _model;

	private Settings _settings;

	private User _user;

	private GameSparksServer _gameSparksServer;

	private SceneLoader _sceneLoader;

	private bool _selectingLanguage;

	public override string AnalyticsScreenName => "settings";

	public void Initialize(SceneLoader sceneLoader)
	{
		_sceneLoader = sceneLoader;
	}

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_model = model;
		_settings = model.Device.Settings;
		_user = model.Device.User;
		_gameSparksServer = _model.GameServer.GameSparksServer;
		CreateLanguageItem(Localization.CurrentCulture, showButton: false);
		int i = 0;
		for (int count = Localization.AvailableCultures.Count; i < count; i++)
		{
			Localization.Culture culture = Localization.AvailableCultures[i];
			if (culture != Localization.CurrentCulture)
			{
				CreateLanguageItem(culture, showButton: true);
			}
		}
		_socialLoginButton.Initialize(model.Game.PopupManager, _settings, _gameSparksServer);
	}

	protected override void OnDestroy()
	{
		if (_gameSparksServer != null)
		{
			_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnGameSparksAuthenticationChanged;
			CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
			gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
			_gameSparksServer = null;
		}
		if (_settings != null)
		{
			_settings.SettingChangedEvent -= OnSettingChanged;
			_settings = null;
		}
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		UpdateContent();
		ToggleLanguageTab(active: false);
		_gameSparksServer.Authenticator.AuthenticationChangedEvent += OnGameSparksAuthenticationChanged;
		CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
		gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Combine(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
		_settings.SettingChangedEvent += OnSettingChanged;
		UpdateSocialBodyText();
	}

	public override void OnCloseClicked()
	{
		if (_selectingLanguage)
		{
			ToggleLanguageTab(active: false);
		}
		else
		{
			base.OnCloseClicked();
		}
	}

	public void OnMusicClicked()
	{
		_settings.ToggleMusic(!_settings.MusicEnabled);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ToggleClick);
		UpdateMusicButton();
	}

	public void OnEffectsClicked()
	{
		_settings.ToggleSoundEffects(!_settings.SoundEffectsEnabled);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ToggleClick);
		UpdateSfxButton();
	}

	public void OnNotificationsClicked()
	{
		_settings.ToggleNotifications(!_settings.NotificationsEnabled);
		SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ToggleClick);
		UpdateNotificationsButton();
	}

	public void OnLanguageClicked()
	{
		ToggleLanguageTab(active: true);
	}

	public void OnOtherGamesClicked()
	{
		_popupManager.RequestPopup(new SSPMenuPopupRequest(SSPMenuPopup.SSPMenuTab.OtherGames));
	}

	public void OnTermsOfServiceClicked()
	{
		Application.OpenURL("https://www.sparklingsociety.net/terms-of-service/");
	}

	public void OnPrivacyPolicyClicked()
	{
		Application.OpenURL("http://www.sparklingsociety.net/privacy-policy/");
	}

	public void OnRateUsClicked()
	{
	}

	public void OnLinkDeviceClicked()
	{
		_popupManager.RequestPopup(new LinkDevicePopupRequest());
	}

	protected override void Closed()
	{
		_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnGameSparksAuthenticationChanged;
		CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
		gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
		_settings.SettingChangedEvent -= OnSettingChanged;
		base.Closed();
	}

	private void CreateLanguageItem(Localization.Culture culture, bool showButton)
	{
		UnityEngine.Object.Instantiate(_languageItemPrefab, _languageItemContentContainer).Initialize(OnLanguageSelected, culture, showButton);
	}

	private void ToggleLanguageTab(bool active)
	{
		if (active)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		}
		else
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		_languageTabRoot.SetActive(active);
		_settingsTabRoot.SetActive(!active);
		_selectingLanguage = active;
		_headerText.LocalizedString = (active ? Localization.Key("settings_language") : Localization.Key("settings_title"));
	}

	private void UpdateContent()
	{
		_languageButtonText.LocalizedString = Localization.CurrentCulture.NativeName;
		_languageButton.gameObject.SetActive(Localization.AvailableCultures.Count > 1);
		_gameVersionText.LocalizedString = Localization.Format(Localization.Key("game_version"), Localization.Literal(CIGGameConstants.VersionString));
		if (string.IsNullOrEmpty(_user.UserKey))
		{
			_playerIdLabel.gameObject.SetActive(value: false);
		}
		else
		{
			_playerIdLabel.gameObject.SetActive(value: true);
			_playerIdLabel.LocalizedString = Localization.Format(Localization.Key("loading_screen.social_your_player_id"), Localization.Literal(_user.UserKey));
		}
		UpdateMusicButton();
		UpdateSfxButton();
		UpdateNotificationsButton();
	}

	private void UpdateMusicButton()
	{
		bool musicEnabled = _settings.MusicEnabled;
		ILocalizedString text = Localization.Format(Localization.Key("music_on_off"), musicEnabled ? Localization.Key("settings_on") : Localization.Key("settings_off"));
		_musicButton.SetState(musicEnabled, text);
	}

	private void UpdateSfxButton()
	{
		bool soundEffectsEnabled = _settings.SoundEffectsEnabled;
		ILocalizedString text = Localization.Format(Localization.Key("sfx_on_off"), soundEffectsEnabled ? Localization.Key("settings_on") : Localization.Key("settings_off"));
		_soundButton.SetState(soundEffectsEnabled, text);
	}

	private void UpdateNotificationsButton()
	{
		bool notificationsEnabled = _settings.NotificationsEnabled;
		ILocalizedString text = Localization.Format(Localization.Key("notifications_on_off"), notificationsEnabled ? Localization.Key("settings_on") : Localization.Key("settings_off"));
		_notificationButton.SetState(notificationsEnabled, text);
	}

	private void OnLanguageSelected(Localization.Culture culture)
	{
		GenericPopupRequest request = new GenericPopupRequest("language_confirm").SetDismissable(dismissable: false).SetTexts(Localization.Key("confirmspend.sure"), Localization.Key("title.switch_to_this_language")).SetGreenOkButton(delegate
		{
			OnLanguageConfirmed(culture);
		})
			.SetRedCancelButton();
		_popupManager.RequestPopup(request);
	}

	private void UpdateSocialBodyText()
	{
		GameSparksAuthentication currentAuthentication = _gameSparksServer.Authenticator.CurrentAuthentication;
		if (_settings.SocialAuthenticationAllowed)
		{
			if (_gameSparksServer.AuthenticationController.HasConnectionFailure)
			{
				_socialBodyText.LocalizedString = Localization.Key("sparksoc.general_error");
			}
			else
			{
				_socialBodyText.LocalizedString = Localization.Format(Localization.Key("DAWA_LOGGED_IN_AS"), Localization.Literal(currentAuthentication.DisplayName));
			}
		}
		else
		{
			_socialBodyText.LocalizedString = Localization.Format(Localization.Key("login_text"), Localization.Literal(CIGGameConstants.SocialServiceName));
		}
	}

	private void OnLanguageConfirmed(Localization.Culture culture)
	{
		_settings.SwitchLanguage(culture);
		_sceneLoader.LoadScene(new GameSceneRequest(_model));
	}

	private void OnGameSparksAuthenticationChanged(GameSparksAuthentication previousAuthentication, GameSparksAuthentication newAuthentication)
	{
		UpdateSocialBodyText();
	}

	private void OnGameSparksAvailable(bool available)
	{
		UpdateSocialBodyText();
	}

	private void OnSettingChanged()
	{
		UpdateSocialBodyText();
	}
}
