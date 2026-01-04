using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksDeviceAuthentication : GameSparksAuthentication
	{
		public GameSparksDeviceAuthentication(CIGGameSparksInstance gameSparksInstance, Settings settings)
			: base(gameSparksInstance, settings)
		{
		}

		public override void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			if (IsAuthenticated)
			{
				EventTools.Fire(onSuccess, value0: true);
			}
			else
			{
				new DeviceAuthenticationRequest(_gameSparksInstance).SetDisplayName("Device").SetScriptData(base.ExtraAuthenticationData).Send(delegate(AuthenticationResponse successResponse)
				{
					HandleAuthenticationResponse(successResponse, onSuccess, onError);
				}, delegate(AuthenticationResponse errorResponse)
				{
					EventTools.Fire(onError, new GameSparksException("AuthenticateDevice", errorResponse));
				});
			}
		}

		public override string ToString()
		{
			return $"GameSparksDeviceAuthentication: {IsAuthenticated}. ID: {base.PlayerId}";
		}
	}
}
