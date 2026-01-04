using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestLinkAccounts : GSTypedRequest<RequestLinkAccounts, LogEventResponse>
	{
		public RequestLinkAccounts(CIGGameSparksInstance instance, string code)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "PlayerLinkingLinkAccounts");
			request.AddString("LinkCode", code);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
