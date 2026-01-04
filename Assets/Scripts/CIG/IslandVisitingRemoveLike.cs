using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class IslandVisitingRemoveLike : GSTypedRequest<IslandVisitingRemoveLike, LogEventResponse>
	{
		public IslandVisitingRemoveLike(CIGGameSparksInstance instance, string userId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "IslandVisitingRemoveLike");
			request.AddString("UserId", userId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
