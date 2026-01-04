using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestFriendsReceiveGift : GSTypedRequest<RequestFriendsReceiveGift, LogEventResponse>
	{
		public RequestFriendsReceiveGift(CIGGameSparksInstance instance, string userId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsReceiveGift");
			request.AddString("UserId", userId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
