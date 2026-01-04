using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestGetFriendCode : GSTypedRequest<RequestGetFriendCode, LogEventResponse>
	{
		public RequestGetFriendCode(CIGGameSparksInstance instance)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsGetFriendCode");
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
