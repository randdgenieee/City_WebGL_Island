using GameSparks.Api.Responses;
using GameSparks.Core;
using System;

namespace CIG
{
	public abstract class GameSparksAuthentication
	{
		private const string UserIdKey = "UserId";

		private const string DisplayNameKey = "DisplayName";

		private const string FriendCodeKey = "FriendCode";

		private const string IsCheaterKey = "IsCheater";

		private static readonly GameSparksJsonSchema AuthenticationResponseSchema = new GameSparksJsonSchema().Field<string>("UserId").Field<string>("DisplayName").Field<bool>("IsCheater");

		private static readonly GameSparksJsonSchema UserDataSchema = new GameSparksJsonSchema().Field<string>("UserId").Field<string>("DisplayName").Field<string>("FriendCode")
			.Field<bool>("IsCheater");

		private readonly Settings _settings;

		protected readonly CIGGameSparksInstance _gameSparksInstance;

		private string _friendCode;

		private bool _generatingFriendCode;

		public virtual bool IsAuthenticated
		{
			get
			{
				if (_gameSparksInstance.Authenticated)
				{
					return _gameSparksInstance.GSPlatform.UserId == PlayerId;
				}
				return false;
			}
		}

		public string PlayerId
		{
			get;
			private set;
		}

		public string UserId
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public string FriendCode
		{
			get
			{
				if (string.IsNullOrEmpty(_friendCode))
				{
					GetFriendCode();
					return "unknown";
				}
				return _friendCode;
			}
			private set
			{
				_friendCode = value;
			}
		}

		public bool IsCheater
		{
			get;
			private set;
		}

		protected GSRequestData ExtraAuthenticationData => new GSRequestData().AddString("PushId", FirebaseManager.MessageToken).AddBoolean("PushNotificationsEnabled", _settings.NotificationsEnabled);

		protected GameSparksAuthentication(CIGGameSparksInstance gameSparksInstance, Settings settings, string displayName = "")
		{
			_gameSparksInstance = gameSparksInstance;
			_settings = settings;
			PlayerId = string.Empty;
			UserId = string.Empty;
			DisplayName = displayName;
			FriendCode = string.Empty;
		}

		public abstract void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError);

		public virtual void ChangeDisplayName(string displayName, Action onSuccess, Action<GameSparksException> onError)
		{
			new RequestUserChangeDisplayName(_gameSparksInstance, displayName).Send(delegate
			{
				DisplayName = displayName;
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("ChangeDisplayName", errorResponse));
			});
		}

		public void UpdateUserData(GSData data)
		{
			string text = UserDataSchema.Validate(data);
			if (!string.IsNullOrEmpty(text))
			{
				throw new GameSparksException("UpdateUserData", text, GSError.SchemaValidationError);
			}
			UserId = data.GetString("UserId");
			DisplayName = data.GetString("DisplayName");
			FriendCode = data.GetString("FriendCode");
			IsCheater = (data.GetBoolean("IsCheater") ?? false);
		}

		protected void HandleAuthenticationResponse(AuthenticationResponse authenticationResponse, Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			HandleResponse(authenticationResponse, authenticationResponse.UserId, onSuccess, onError);
		}

		protected void HandleRegistrationResponse(RegistrationResponse registrationResponse, Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			HandleResponse(registrationResponse, registrationResponse.UserId, onSuccess, onError);
		}

		private void GetFriendCode()
		{
			if (!_generatingFriendCode)
			{
				_generatingFriendCode = true;
				new RequestGetFriendCode(_gameSparksInstance).Send(delegate(LogEventResponse successResponse)
				{
					_generatingFriendCode = false;
					if (successResponse.HasErrors)
					{
						string @string = successResponse.Errors.GetString("FriendCode");
						GameSparksUtils.LogGameSparksError(new GameSparksException("GetFriendCode", @string, GSError.FriendCodeGenerationFailed));
					}
					else
					{
						FriendCode = successResponse.ScriptData.GetString("FriendCode");
					}
				}, delegate(LogEventResponse errorResponse)
				{
					_generatingFriendCode = false;
					GameSparksUtils.LogGameSparksError(new GameSparksException("GetFriendCode", errorResponse));
				});
			}
		}

		private void HandleResponse(GSTypedResponse response, string playerId, Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			string text = AuthenticationResponseSchema.Validate(response.ScriptData);
			if (!string.IsNullOrEmpty(text))
			{
				EventTools.Fire(onError, new GameSparksException("HandleAuthenticationResponse", text, GSError.SchemaValidationError));
				return;
			}
			PlayerId = playerId;
			UserId = response.ScriptData.GetString("UserId");
			DisplayName = response.ScriptData.GetString("DisplayName");
			FriendCode = response.ScriptData.GetString("FriendCode");
			IsCheater = (response.ScriptData.GetBoolean("IsCheater") ?? false);
			EventTools.Fire(onSuccess, IsAuthenticated);
		}
	}
}
