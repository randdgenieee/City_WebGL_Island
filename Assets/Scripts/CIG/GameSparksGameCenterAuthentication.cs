using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksGameCenterAuthentication : GameSparksAuthentication
	{
		public string GameCenterId
		{
			get;
			set;
		}

		public string PublicKeyUrl
		{
			get;
			set;
		}

		public string Signature
		{
			get;
			set;
		}

		public string Salt
		{
			get;
			set;
		}

		public long Timestamp
		{
			get;
			set;
		}

		public GameSparksGameCenterAuthentication(CIGGameSparksInstance gameSparksInstance, Settings settings, string displayName, string playerId, string publicKeyUrl, string signature, string salt, long timestamp)
			: base(gameSparksInstance, settings, displayName)
		{
			GameCenterId = playerId;
			PublicKeyUrl = publicKeyUrl;
			Signature = signature;
			Salt = salt;
			Timestamp = timestamp;
		}

		public override void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			if (IsAuthenticated)
			{
				EventTools.Fire(onSuccess, value0: true);
			}
			else
			{
				new GameCenterConnectRequest(_gameSparksInstance).SetExternalPlayerId(GameCenterId).SetDisplayName(base.DisplayName).SetPublicKeyUrl(PublicKeyUrl)
					.SetSignature(Signature)
					.SetSalt(Salt)
					.SetTimestamp(Timestamp)
					.SetDoNotLinkToCurrentPlayer(doNotLinkToCurrentPlayer: true)
					.SetScriptData(base.ExtraAuthenticationData)
					.Send(delegate(AuthenticationResponse successResponse)
					{
						HandleAuthenticationResponse(successResponse, onSuccess, onError);
					}, delegate(AuthenticationResponse errorResponse)
					{
						EventTools.Fire(onError, new GameSparksException("AuthenticateGameCenter", errorResponse));
					});
			}
		}

		public override string ToString()
		{
			return $"GameSparksGameCenterAuthentication: {IsAuthenticated}. ID: {base.PlayerId}";
		}
	}
}
