using GameSparks.Core;
using System;

namespace CIG
{
	public class GameSparksException : Exception
	{
		private const string BaseFormat = "[CIG-GS] Encountered GameSparks error ('{0}') while doing {1}. {2}";

		private const string ResponseFormat = "Full response:\n\n{0}";

		private string _message;

		public GSError Error
		{
			get;
			private set;
		}

		public GameSparksException(string action, GSTypedResponse response)
		{
			Error = ErrorFromResponse(response);
			_message = $"[CIG-GS] Encountered GameSparks error ('{ErrorToString(Error)}') while doing {action}. {ResponseToString(response)}";
		}

		public GameSparksException(string action, string message, GSError error)
		{
			Error = error;
			_message = $"[CIG-GS] Encountered GameSparks error ('{ErrorToString(Error)}') while doing {action}. {message}";
		}

		public GameSparksException(string action, GSError error)
		{
			Error = error;
			_message = string.Format("[CIG-GS] Encountered GameSparks error ('{0}') while doing {1}. {2}", ErrorToString(Error), action, "");
		}

		public GameSparksException(string action)
		{
			Error = GSError.Other;
			_message = string.Format("[CIG-GS] Encountered GameSparks error ('{0}') while doing {1}. {2}", ErrorToString(Error), action, "");
		}

		public override string ToString()
		{
			return _message;
		}

		private static string ResponseToString(GSTypedResponse response)
		{
			return $"Full response:\n\n{response.JSONString}";
		}

		private static string ErrorToString(GSError error)
		{
			switch (error)
			{
			case GSError.Other:
				return "Unknown";
			case GSError.InvalidUserDetails:
				return "Invalid User Details";
			case GSError.NoConnection:
				return "No Connection";
			case GSError.SchemaValidationError:
				return "JsonSchema Validation Error";
			case GSError.NotAuthenticated:
				return "Not authenticated";
			case GSError.ExternalAuthenticationFailed:
				return "Authentication with external provider failed.";
			default:
				return "Unknown";
			}
		}

		private static GSError ErrorFromResponse(GSTypedResponse response)
		{
			if (!response.HasErrors)
			{
				return GSError.Other;
			}
			if (response.Errors.ContainsKey("DETAILS") && response.Errors.GetString("DETAILS") == "UNRECOGNISED")
			{
				return GSError.InvalidUserDetails;
			}
			if (response.Errors.ContainsKey("error") && response.Errors.GetString("error") == "timeout")
			{
				return GSError.NoConnection;
			}
			if (response.Errors.ContainsKey("authentication") && response.Errors.GetString("authentication") == "NOTAUTHORIZED")
			{
				return GSError.NotAuthenticated;
			}
			if (response.Errors.ContainsKey("code") && response.Errors.GetString("code") == "NOTAUTHENTICATED")
			{
				return GSError.ExternalAuthenticationFailed;
			}
			if ((response.Errors.ContainsKey("signature") && response.Errors.GetString("signature") == "NOTAUTHENTICATED") || (response.Errors.ContainsKey("externalPlayerId") && response.Errors.GetString("externalPlayerId") == "NOTAUTHENTICATED"))
			{
				return GSError.ExternalAuthenticationFailed;
			}
			if (response.Errors.ContainsKey("Result"))
			{
				if (response.Errors.GetString("Result") == "AlreadyLinkedToSameUser")
				{
					return GSError.AlreadyLinkedToSameUser;
				}
				if (response.Errors.GetString("Result") == "InvalidCode")
				{
					return GSError.InvalidCode;
				}
				return GSError.Other;
			}
			if (response.Errors.ContainsKey("INVALID_LINKCODE") && response.Errors.GetString("INVALID_LINKCODE") == "No user found")
			{
				return GSError.InvalidCode;
			}
			if (response.Errors.ContainsKey("FriendData") && response.Errors.GetString("FriendData") == "Invalid friendcode")
			{
				return GSError.InvalidCode;
			}
			return GSError.Other;
		}
	}
}
