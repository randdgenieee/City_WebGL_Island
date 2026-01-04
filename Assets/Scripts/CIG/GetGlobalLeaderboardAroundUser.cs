using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class GetGlobalLeaderboardAroundUser : GSTypedRequest<GetGlobalLeaderboardAroundUser, LogEventResponse>
	{
		public GetGlobalLeaderboardAroundUser(CIGGameSparksInstance instance, int padding)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "GetGlobalLeaderboardAroundUser");
			request.AddNumber("Padding", padding);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
