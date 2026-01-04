namespace SparklingGameCenterSDK
{
	public static class SparklingGameCenterSDK
	{
		public delegate void AuthenticateSuccessCallback();

		public delegate void SignatureVerificationSuccessCallback(string publicKeyUrl, string signature, string salt, ulong timestamp);

		public delegate void ErrorCallback(string error);

		private const string PlatformUnsupportedMessage = "SparklingGameCenterSDK is an iOS-only feature, and is not supported on this platform.";

		private static bool _isAuthenticating;

		private static bool _isGettingSignatureValidation;

		public static void AuthenticatePlayer(AuthenticateSuccessCallback success, ErrorCallback error)
		{
			if (!_isAuthenticating)
			{
				error?.Invoke("SparklingGameCenterSDK is an iOS-only feature, and is not supported on this platform.");
			}
		}

		public static void GetSignatureVerification(SignatureVerificationSuccessCallback success, ErrorCallback error)
		{
			if (!_isGettingSignatureValidation)
			{
				error?.Invoke("SparklingGameCenterSDK is an iOS-only feature, and is not supported on this platform.");
			}
		}

		public static string GetDisplayName()
		{
			return string.Empty;
		}

		public static string GetPlayerId()
		{
			return string.Empty;
		}

		public static bool GetIsAuthenticated()
		{
			return false;
		}
	}
}
