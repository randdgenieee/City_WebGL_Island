using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestGetFriendsList : GSTypedRequest<RequestGetFriendsList, LogEventResponse>
	{
		public RequestGetFriendsList(CIGGameSparksInstance instance)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsGetFriendsList");
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
