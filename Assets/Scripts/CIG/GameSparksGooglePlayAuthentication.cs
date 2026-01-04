using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksGooglePlayAuthentication : GameSparksAuthentication
	{
		public string ServerAuthCode
		{
			get;
			private set;
		}

		public GameSparksGooglePlayAuthentication(CIGGameSparksInstance gameSparksInstance, Settings settings, string displayName, string authCode)
			: base(gameSparksInstance, settings, displayName)
		{
			ServerAuthCode = authCode;
		}

		public override void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			if (IsAuthenticated)
			{
				EventTools.Fire(onSuccess, value0: true);
			}
			else
			{
				new GooglePlayConnectRequest(_gameSparksInstance).SetCode(ServerAuthCode).SetRedirectUri("http://www.gamesparks.com/oauth2callback").SetDisplayName(base.DisplayName)
					.SetDoNotLinkToCurrentPlayer(doNotLinkToCurrentPlayer: true)
					.SetScriptData(base.ExtraAuthenticationData)
					.Send(delegate(AuthenticationResponse successResponse)
					{
						HandleAuthenticationResponse(successResponse, onSuccess, onError);
					}, delegate(AuthenticationResponse errorResponse)
					{
						EventTools.Fire(onError, new GameSparksException("AuthenticateGooglePlay", errorResponse));
					});
			}
		}

		public override string ToString()
		{
			return $"GameSparksGooglePlayAuthentication: {IsAuthenticated}. ID: {base.PlayerId}";
		}
	}
}
