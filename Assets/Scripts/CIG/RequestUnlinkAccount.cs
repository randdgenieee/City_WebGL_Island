using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestUnlinkAccount : GSTypedRequest<RequestUnlinkAccount, LogEventResponse>
	{
		public RequestUnlinkAccount(CIGGameSparksInstance instance)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "PlayerLinkingUnlinkAccount");
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
