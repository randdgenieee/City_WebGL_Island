using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class IslandVisitingAddLike : GSTypedRequest<IslandVisitingAddLike, LogEventResponse>
	{
		public IslandVisitingAddLike(CIGGameSparksInstance instance, string userId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "IslandVisitingAddLike");
			request.AddString("UserId", userId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
