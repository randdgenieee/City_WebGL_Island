using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestLogout : GSTypedRequest<RequestLogout, LogEventResponse>
	{
		public RequestLogout(CIGGameSparksInstance instance, string pushId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "Logout");
			request.AddString("PushId", pushId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
