using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestCloudStoragePullMetadata : GSTypedRequest<RequestCloudStoragePullMetadata, LogEventResponse>
	{
		public RequestCloudStoragePullMetadata(CIGGameSparksInstance instance, string deviceId)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "CloudStoragePullMetadata");
			request.AddString("DeviceId", deviceId);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
