using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestFriendsGiveGift : GSTypedRequest<RequestFriendsGiveGift, LogEventResponse>
	{
		public RequestFriendsGiveGift(CIGGameSparksInstance instance, string userId, Currencies currencies)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsGiveGift");
			request.AddString("UserId", userId);
			request.AddObject("Currencies", currencies.ToGSData());
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
