using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestDeclineFriendRequest : GSTypedRequest<RequestDeclineFriendRequest, LogEventResponse>
	{
		public RequestDeclineFriendRequest(CIGGameSparksInstance instance, string friendCode)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsDeclineFriendRequest");
			request.AddString("FriendCode", friendCode);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
