using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestUserChangeDisplayName : GSTypedRequest<RequestUserChangeDisplayName, LogEventResponse>
	{
		public RequestUserChangeDisplayName(CIGGameSparksInstance instance, string displayName)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "UserChangeDisplayName");
			request.AddString("DisplayName", displayName);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
