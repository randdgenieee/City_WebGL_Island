using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksUsernameAuthentication : GameSparksAuthentication
	{
		private const int MinUserNameLength = 3;

		private const int MaxUserNameLength = 25;

		private const int MinPasswordLength = 5;

		public string UserName
		{
			get;
			private set;
		}

		public string Password
		{
			get;
			private set;
		}

		public GameSparksUsernameAuthentication(CIGGameSparksInstance gameSparksInstance, Settings settings, string userName, string password)
			: base(gameSparksInstance, settings)
		{
			UserName = userName;
			Password = password;
		}

		public override void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			if (IsAuthenticated)
			{
				EventTools.Fire(onSuccess, value0: true);
			}
			else
			{
				new AuthenticationRequest(_gameSparksInstance).SetUserName(UserName).SetPassword(Password).SetScriptData(base.ExtraAuthenticationData)
					.Send(delegate(AuthenticationResponse successResponse)
					{
						HandleAuthenticationResponse(successResponse, onSuccess, onError);
					}, delegate(AuthenticationResponse errorResponse)
					{
						EventTools.Fire(onError, new GameSparksException("AuthenticateUserName", errorResponse));
					});
			}
		}

		public void Register(string displayName, Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			new RegistrationRequest(_gameSparksInstance).SetDisplayName(displayName).SetUserName(UserName).SetPassword(Password)
				.Send(delegate(RegistrationResponse successResponse)
				{
					HandleRegistrationResponse(successResponse, onSuccess, onError);
				}, delegate(RegistrationResponse errorResponse)
				{
					EventTools.Fire(onError, new GameSparksException("RegisterUserName", errorResponse));
				});
		}

		public static bool IsValidUsername(string username)
		{
			int length = username.Length;
			if (length >= 3)
			{
				return length <= 25;
			}
			return false;
		}

		public static bool IsValidPassword(string password)
		{
			return password.Length >= 5;
		}

		public override string ToString()
		{
			return $"GameSparksUsernameAuthentication: {IsAuthenticated}. ID: {base.PlayerId}";
		}
	}
}
