using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestAcountLinkCode : GSTypedRequest<RequestAcountLinkCode, LogEventResponse>
	{
		public RequestAcountLinkCode(CIGGameSparksInstance instance)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "PlayerLinkingRequestLinkCode");
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
