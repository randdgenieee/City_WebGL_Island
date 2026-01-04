using System;

namespace CIG
{
    public class GameSparksAuthenticationController
    {
        private enum AuthenticationType
        {
            None,
            Device,
            Username,
            GooglePlayGames,
            GameCenter
        }

        private delegate void ErrorHandler(GSError error);

        private readonly Settings _settings;

        private readonly GameSparksAuthenticator _authenticator;

        private readonly GameCenterService _gameCenterService;

        private readonly GameSparksUser _gameSparksUser;

        private AuthenticationType _autoAuthenticationType;

        public bool HasConnectionFailure
        {
            get
            {
                if (_settings.AuthenticationAllowed)
                {
                    return !_authenticator.CurrentAuthentication.IsAuthenticated;
                }
                return false;
            }
        }

        private bool IsAutoAuthenticated
        {
            get
            {
                switch (_autoAuthenticationType)
                {
                    case AuthenticationType.None:
                        return _authenticator.CurrentAuthentication is GameSparksNoAuthentication;
                    case AuthenticationType.Device:
                        return _authenticator.IsAuthenticatedWith<GameSparksDeviceAuthentication>();
                    case AuthenticationType.Username:
                        return _authenticator.IsAuthenticatedWith<GameSparksUsernameAuthentication>();
                    case AuthenticationType.GooglePlayGames:
                        return _authenticator.IsAuthenticatedWith<GameSparksGooglePlayAuthentication>();
                    case AuthenticationType.GameCenter:
                        return _authenticator.IsAuthenticatedWith<GameSparksGameCenterAuthentication>();
                    default:
                        return false;
                }
            }
        }

        public GameSparksAuthenticationController(Settings settings, GameSparksAuthenticator authenticator, GameCenterService gameCenterService, GameSparksUser gameSparksUser)
        {
            _settings = settings;
            _authenticator = authenticator;
            _gameCenterService = gameCenterService;
            _gameSparksUser = gameSparksUser;
            _settings.SettingChangedEvent += OnSettingsChanged;
            DetermineAuthenticationType(_settings.AuthenticationAllowed, _settings.SocialAuthenticationAllowed);
        }

        public void AutoAuthenticate(Action onSuccess, Action onError)
        {
            if (!IsAutoAuthenticated)
            {
                ErrorHandler errorHandler = delegate (GSError error)
                {
                    OnAutoAuthenticateError(error);
                    EventTools.Fire(onError);
                };
                switch (_autoAuthenticationType)
                {
                    case AuthenticationType.None:
                        _authenticator.Logout(onSuccess, delegate (GameSparksException exception)
                        {
                            AuthenticationError(exception, errorHandler);
                        });
                        break;
                    case AuthenticationType.Device:
                        AuthenticateWithDevice(onSuccess, errorHandler);
                        break;
                    case AuthenticationType.Username:
                        AuthenticateWithUsername(onSuccess, errorHandler);
                        break;
                    case AuthenticationType.GooglePlayGames:
                        AuthenticateWithGooglePlayGames(onSuccess, errorHandler);
                        break;
                    case AuthenticationType.GameCenter:
                        AuthenticateWithGameCenter(onSuccess, errorHandler);
                        break;
                    default:
                        EventTools.Fire(onSuccess);
                        break;
                }
            }
            else
            {
                EventTools.Fire(onSuccess);
            }
        }

        public void ToggleSocialAuthenticationAllowed(bool on, Action onSuccess, Action onError)
        {
            DetermineAuthenticationType(authenticationAllowed: true, on);
            AutoAuthenticate(delegate
            {
                _settings.ToggleSocialAuthenticationAllowed(on);
                DetermineAuthenticationType(_settings.AuthenticationAllowed, _settings.SocialAuthenticationAllowed);
                EventTools.Fire(onSuccess);
            }, delegate
            {
                _settings.ToggleSocialAuthenticationAllowed(on: false);
                DetermineAuthenticationType(_settings.AuthenticationAllowed, _settings.SocialAuthenticationAllowed);
                EventTools.Fire(onError);
            });
        }

        public bool IsSocialAuthentication(GameSparksAuthentication authentication)
        {
            if (!(authentication is GameSparksNoAuthentication))
            {
                return !(authentication is GameSparksDeviceAuthentication);
            }
            return false;
        }

        private void OnAutoAuthenticateError(GSError error)
        {
            if (error == GSError.InvalidUserDetails)
            {
                _settings.SettingChangedEvent -= OnSettingsChanged;
                _settings.ToggleAuthenticationAllowed(on: false);
                _settings.ToggleSocialAuthenticationAllowed(on: false);
                _settings.SettingChangedEvent += OnSettingsChanged;
                DetermineAuthenticationType(authenticationAllowed: false, socialAuthenticationAllowed: false);
            }
        }

        private void OnSettingsChanged()
        {
            DetermineAuthenticationType(_settings.AuthenticationAllowed, _settings.SocialAuthenticationAllowed);
        }

        private void AuthenticateWithDevice(Action onSuccess, ErrorHandler onError)
        {
            _authenticator.AuthenticateDevice(delegate
            {
                EventTools.Fire(onSuccess);
            }, delegate (GameSparksException err)
            {
                AuthenticationError(err, onError);
            });
        }

        private void AuthenticateWithUsername(Action onSuccess, ErrorHandler onError)
        {
            GameSparksUtils.LoadAccountFromStorage(out string username, out string password);
            if (_authenticator.IsAuthenticatedWith<GameSparksDeviceAuthentication>())
            {
                _gameSparksUser.UnlinkPlayer(delegate
                {
                    _authenticator.AuthenticateUserName(username, password, delegate
                    {
                        EventTools.Fire(onSuccess);
                    }, delegate (GameSparksException err)
                    {
                        AuthenticationError(err, onError);
                    });
                }, delegate (GameSparksException err)
                {
                    AuthenticationError(err, onError);
                });
            }
            else
            {
                _authenticator.AuthenticateUserName(username, password, delegate
                {
                    EventTools.Fire(onSuccess);
                }, delegate (GameSparksException err)
                {
                    AuthenticationError(err, onError);
                });
            }
        }

        private void AuthenticateWithGooglePlayGames(Action onSuccess, ErrorHandler onError)
        {
        }

        private void AuthenticateWithGameCenter(Action onSuccess, ErrorHandler onError)
        {
            _gameCenterService.GetSignatureVerification(delegate (string publicKeyUrl, string signature, string salt, ulong timestamp)
            {
                if (_authenticator.IsAuthenticatedWith<GameSparksDeviceAuthentication>())
                {
                    _gameSparksUser.UnlinkPlayer(delegate
                    {
                        _authenticator.AuthenticateGameCenter(_gameCenterService.GetDisplayName(), _gameCenterService.GetPlayerId(), publicKeyUrl, signature, salt, (long)timestamp, delegate
                        {
                            EventTools.Fire(onSuccess);
                        }, delegate (GameSparksException err)
                        {
                            AuthenticationError(err, onError);
                        });
                    }, delegate (GameSparksException err)
                    {
                        AuthenticationError(err, onError);
                    });
                }
                else
                {
                    _authenticator.AuthenticateGameCenter(_gameCenterService.GetDisplayName(), _gameCenterService.GetPlayerId(), publicKeyUrl, signature, salt, (long)timestamp, delegate
                    {
                        EventTools.Fire(onSuccess);
                    }, delegate (GameSparksException err)
                    {
                        AuthenticationError(err, onError);
                    });
                }
            }, delegate (string err)
            {
                AuthenticationError(err, onError);
            });
        }

        private void AuthenticationError(string error, ErrorHandler onError)
        {
            onError?.Invoke(GSError.Other);
        }

        private void AuthenticationError(GameSparksException error, ErrorHandler onError)
        {
            GameSparksUtils.LogGameSparksError(error);
            onError?.Invoke(error.Error);
        }

        private void DetermineAuthenticationType(bool authenticationAllowed, bool socialAuthenticationAllowed)
        {
            if (!authenticationAllowed)
            {
                _autoAuthenticationType = AuthenticationType.None;
            }
            else if (socialAuthenticationAllowed)
            {
                _autoAuthenticationType = AuthenticationType.GooglePlayGames;
            }
            else
            {
                _autoAuthenticationType = AuthenticationType.Device;
            }
        }
    }
}
