using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksAuthenticator
	{
		public delegate void AuthenticationChangedEventHandler(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication);

		private readonly CIGGameSparksInstance _gameSparksInstance;

		private readonly Settings _settings;

		public GameSparksAuthentication CurrentAuthentication
		{
			get;
			private set;
		}

		public event AuthenticationChangedEventHandler AuthenticationChangedEvent;

		private void FireAuthenticationChangedEvent(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
		{
			this.AuthenticationChangedEvent?.Invoke(newAuthentication, previousAuthentication);
		}

		public GameSparksAuthenticator(CIGGameSparksInstance gameSparksInstance, Settings settings)
		{
			_gameSparksInstance = gameSparksInstance;
			_settings = settings;
			CurrentAuthentication = new GameSparksNoAuthentication(_settings);
		}

		public void RegisterUserName(string displayName, string userName, string password, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			GameSparksUsernameAuthentication authentication = new GameSparksUsernameAuthentication(_gameSparksInstance, _settings, userName, password);
			authentication.Register(displayName, delegate
			{
				AuthenticateWith(authentication, onSuccess, onError);
			}, delegate(GameSparksException exc)
			{
				EventTools.Fire(onError, exc);
			});
		}

		public void AuthenticateDevice(Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			AuthenticateWith(new GameSparksDeviceAuthentication(_gameSparksInstance, _settings), onSuccess, onError);
		}

		public void AuthenticateUserName(string userName, string password, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			AuthenticateWith(new GameSparksUsernameAuthentication(_gameSparksInstance, _settings, userName, password), onSuccess, onError);
		}

		public void AuthenticateGooglePlayGames(string displayName, string authCode, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			AuthenticateWith(new GameSparksGooglePlayAuthentication(_gameSparksInstance, _settings, displayName, authCode), onSuccess, onError);
		}

		public void AuthenticateGameCenter(string displayName, string gameCenterId, string publicKeyUrl, string signature, string salt, long timestamp, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			AuthenticateWith(new GameSparksGameCenterAuthentication(_gameSparksInstance, _settings, displayName, gameCenterId, publicKeyUrl, signature, salt, timestamp), onSuccess, onError);
		}

		public void Logout(Action onSuccess, Action<GameSparksException> onError)
		{
			LogoutInternal(delegate
			{
				AuthenticateWith(new GameSparksNoAuthentication(_settings), delegate
				{
					_gameSparksInstance.Reset();
					EventTools.Fire(onSuccess);
				}, delegate(GameSparksException exception)
				{
					EventTools.Fire(onError, exception);
				});
			}, onError);
		}

		public void SetDisplayName(string displayName, Action success, Action<GameSparksException> error)
		{
			if (!CurrentAuthentication.IsAuthenticated)
			{
				EventTools.Fire(error, new GameSparksException("Setting display name", GSError.NotAuthenticated));
			}
			else
			{
				CurrentAuthentication.ChangeDisplayName(displayName, success, error);
			}
		}

		public bool IsAuthenticatedWith<T>() where T : GameSparksAuthentication
		{
			if (CurrentAuthentication.IsAuthenticated)
			{
				return CurrentAuthentication is T;
			}
			return false;
		}

		private void AuthenticateWith(GameSparksAuthentication authentication, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			if (CurrentAuthentication.IsAuthenticated)
			{
				LogoutInternal(delegate
				{
					Login(authentication, onSuccess, onError);
				}, onError);
			}
			else
			{
				Login(authentication, onSuccess, onError);
			}
		}

		private void Login(GameSparksAuthentication authentication, Action<GameSparksAuthentication> onSuccess, Action<GameSparksException> onError)
		{
			authentication.Authenticate(delegate
			{
				GameSparksAuthentication currentAuthentication = CurrentAuthentication;
				CurrentAuthentication = authentication;
				FireAuthenticationChangedEvent(CurrentAuthentication, currentAuthentication);
				EventTools.Fire(onSuccess, CurrentAuthentication);
			}, delegate(GameSparksException exc)
			{
				FireAuthenticationChangedEvent(CurrentAuthentication, CurrentAuthentication);
				EventTools.Fire(onError, exc);
			});
		}

		private void LogoutInternal(Action onSuccess, Action<GameSparksException> onError)
		{
			if (string.IsNullOrEmpty(FirebaseManager.MessageToken))
			{
				EventTools.Fire(onSuccess);
			}
			else
			{
				new RequestLogout(_gameSparksInstance, FirebaseManager.MessageToken).Send(delegate
				{
					EventTools.Fire(onSuccess);
				}, delegate(LogEventResponse errorResponse)
				{
					EventTools.Fire(onError, new GameSparksException("LogoutInternal", errorResponse));
				});
			}
		}
	}
}
