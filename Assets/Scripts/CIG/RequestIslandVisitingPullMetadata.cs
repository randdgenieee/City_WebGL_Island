using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestIslandVisitingPullMetadata : GSTypedRequest<RequestIslandVisitingPullMetadata, LogEventResponse>
	{
		public RequestIslandVisitingPullMetadata(CIGGameSparksInstance instance, string userId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "IslandVisitingPullMetadata");
			request.AddString("UserId", userId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
