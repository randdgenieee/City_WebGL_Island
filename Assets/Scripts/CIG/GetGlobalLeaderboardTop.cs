using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class GetGlobalLeaderboardTop : GSTypedRequest<GetGlobalLeaderboardTop, LogEventResponse>
	{
		public GetGlobalLeaderboardTop(CIGGameSparksInstance instance, int maxEntries)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "GetGlobalLeaderboardTop");
			request.AddNumber("MaxItems", maxEntries);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
