using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class GetCountryLeaderboardAroundUser : GSTypedRequest<GetCountryLeaderboardAroundUser, LogEventResponse>
	{
		public GetCountryLeaderboardAroundUser(CIGGameSparksInstance instance, int padding, string country)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "GetCountryLeaderboardAroundUser");
			request.AddNumber("Padding", padding);
			request.AddString("Country", country);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
