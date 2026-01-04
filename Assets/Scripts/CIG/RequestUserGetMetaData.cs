using GameSparks.Api.Responses;
using GameSparks.Core;

namespace CIG
{
	public class RequestUserGetMetaData : GSTypedRequest<RequestUserGetMetaData, LogEventResponse>
	{
		public RequestUserGetMetaData(CIGGameSparksInstance instance, string linkCode)
			: base((GSInstance)instance, "LogEventRequest")
		{
			request.AddString("eventKey", "UserGetMetaData");
			request.AddString("LinkCode", linkCode);
		}

		protected override GSTypedResponse BuildResponse(GSObject response)
		{
			return new LogEventResponse(response);
		}
	}
}
