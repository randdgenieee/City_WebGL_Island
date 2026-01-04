using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class GetCountryLeaderboardTop : GSTypedRequest<GetCountryLeaderboardTop, LogEventResponse>
	{
		public GetCountryLeaderboardTop(CIGGameSparksInstance instance, int maxEntries, string country)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "GetCountryLeaderboardTop");
			request.AddNumber("MaxItems", maxEntries);
			request.AddString("Country", country);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
