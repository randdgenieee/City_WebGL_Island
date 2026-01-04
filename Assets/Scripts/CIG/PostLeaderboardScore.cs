using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class PostLeaderboardScore : GSTypedRequest<PostLeaderboardScore, LogEventResponse>
	{
		public PostLeaderboardScore(CIGGameSparksInstance instance, int score, int population, int level, string country)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "PostLeaderboardScore");
			request.AddNumber("Score", score);
			request.AddNumber("Population", population);
			request.AddNumber("Level", level);
			request.AddString("Country", country);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
