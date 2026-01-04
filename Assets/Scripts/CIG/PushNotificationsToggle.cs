using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class PushNotificationsToggle : GSTypedRequest<PushNotificationsToggle, LogEventResponse>
	{
		public PushNotificationsToggle(CIGGameSparksInstance instance, string pushId, bool enabled)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "PushNotificationsToggle");
			request.AddString("PushId", pushId);
			request.AddString("Enabled", enabled ? "true" : "false");
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
