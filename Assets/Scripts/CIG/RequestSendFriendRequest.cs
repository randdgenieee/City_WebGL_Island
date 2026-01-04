using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestSendFriendRequest : GSTypedRequest<RequestSendFriendRequest, LogEventResponse>
	{
		public RequestSendFriendRequest(CIGGameSparksInstance instance, string friendCode)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsSendFriendRequest");
			request.AddString("FriendCode", friendCode);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
