using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SocialLoginButton : MonoBehaviour
	{
		private enum State
		{
			LoggedIn,
			LoggedOut,
			Uninteractable
		}

		[SerializeField]
		private Button _button;

		[SerializeField]
		private Image _buttonImage;

		[SerializeField]
		private Sprite _loggedInButtonSprite;

		[SerializeField]
		private Sprite _loggedOutButtonSprite;

		[SerializeField]
		private Sprite _disabledButtonSprite;

		[SerializeField]
		private LocalizedText _text;

		[SerializeField]
		private Color _loggedInTextColor;

		[SerializeField]
		private Color _loggedOutTextColor;

		[SerializeField]
		private Color _disabledTextColor;

		[SerializeField]
		private Shadow _textShadow;

		[SerializeField]
		private Color _loggedInShadowColor;

		[SerializeField]
		private Color _loggedOutShadowColor;

		[SerializeField]
		private Color _disabledShadowColor;

		private static State _state;

		private static bool _waitingForCallback;

		private PopupManager _popupManager;

		private Settings _settings;

		private GameSparksServer _gameSparksServer;

		public void Initialize(PopupManager popupManager, Settings settings, GameSparksServer gameSparksServer)
		{
			_popupManager = popupManager;
			_settings = settings;
			_gameSparksServer = gameSparksServer;
			_text.LocalizedString = Localization.Literal(CIGGameConstants.SocialServiceName);
			_gameSparksServer.Authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
			UpdateState();
		}

		private void OnEnable()
		{
			UpdateVisuals();
		}

		private void OnDestroy()
		{
			_waitingForCallback = false;
			if (_gameSparksServer != null)
			{
				_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
				_gameSparksServer = null;
			}
		}

		public void OnClicked()
		{
			_waitingForCallback = true;
			UpdateState();
			if (_settings.SocialAuthenticationAllowed)
			{
				GenericPopupRequest request = new GenericPopupRequest("logout_confirm").SetTexts(Localization.Key("settings_logout"), Localization.Key("logout_confirm")).SetDismissable(dismissable: false, OnDisableSocialAuthenticationCancelled).SetGreenOkButton(OnDisableSocialAuthenticationConfirmed)
					.SetRedCancelButton(OnDisableSocialAuthenticationCancelled);
				_popupManager.RequestPopup(request);
			}
			else
			{
				_gameSparksServer.AuthenticationController.ToggleSocialAuthenticationAllowed(on: true, OnSocialAuthenticationToggled, OnSocialAuthenticationToggled);
			}
		}

		private void UpdateState()
		{
			if (_waitingForCallback || !CIGGameConstants.SocialServiceSupported)
			{
				_state = State.Uninteractable;
			}
			else if (_settings.SocialAuthenticationAllowed)
			{
				_state = State.LoggedIn;
			}
			else
			{
				_state = State.LoggedOut;
			}
			UpdateVisuals();
		}

		private void UpdateVisuals()
		{
			switch (_state)
			{
			case State.LoggedIn:
				_button.interactable = true;
				_buttonImage.sprite = _loggedInButtonSprite;
				_text.TextField.color = _loggedInTextColor;
				_textShadow.effectColor = _loggedInShadowColor;
				break;
			case State.LoggedOut:
				_button.interactable = true;
				_buttonImage.sprite = _loggedOutButtonSprite;
				_text.TextField.color = _loggedOutTextColor;
				_textShadow.effectColor = _loggedOutShadowColor;
				break;
			case State.Uninteractable:
				_button.interactable = false;
				_buttonImage.sprite = _disabledButtonSprite;
				_text.TextField.color = _disabledTextColor;
				_textShadow.effectColor = _disabledShadowColor;
				break;
			default:
				UnityEngine.Debug.LogWarningFormat("Can't set state '{0}'.", _state);
				break;
			}
		}

		private void OnLoginFinished(bool loggedIn)
		{
			_waitingForCallback = false;
			UpdateState();
		}

		private void OnDisableSocialAuthenticationConfirmed()
		{
			_gameSparksServer.AuthenticationController.ToggleSocialAuthenticationAllowed(on: false, OnSocialAuthenticationToggled, OnSocialAuthenticationToggled);
		}

		private void OnDisableSocialAuthenticationCancelled()
		{
			_waitingForCallback = false;
			UpdateState();
		}

		private void OnSocialAuthenticationToggled()
		{
			_waitingForCallback = false;
			UpdateState();
		}

		private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			UpdateState();
		}
	}
}
