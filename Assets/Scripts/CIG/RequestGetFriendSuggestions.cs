using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestGetFriendSuggestions : GSTypedRequest<RequestGetFriendSuggestions, LogEventResponse>
	{
		public RequestGetFriendSuggestions(CIGGameSparksInstance instance, int amount, int level)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "FriendsGetFriendSuggestions");
			request.AddNumber("Amount", amount);
			request.AddNumber("Level", level);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
