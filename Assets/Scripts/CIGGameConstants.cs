using System;
using UnityEngine;

public static class CIGGameConstants
{
	public const string FacebookUrl = "https://www.facebook.com/SparklingSocietyCityBuildingGames/";

	public const string TwitterUrl = "https://twitter.com/sprklingsociety";

	public const string SparklingSocietyUrl = "http://www.sparklingsociety.net/";

	public const string TermsOfServiceUrl = "https://www.sparklingsociety.net/terms-of-service/";

	public const string PrivacyPolicyUrl = "http://www.sparklingsociety.net/privacy-policy/";

	public const string FaqUrl = "https://www.sparklingsociety.net/cig5-forum";

	public static readonly DateTime UnixOriginTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public const float ReferenceDPI = 96f;

	public static float CurrentDPI
	{
		get
		{
			if (!(Screen.dpi <= 0f))
			{
				return Screen.dpi;
			}
			return 96f;
		}
	}

	public static string VersionString => string.Format("{0} ({1}Build {2})", NativeGameVersion.Version, UnityEngine.Debug.isDebugBuild ? "Debug " : "", NativeGameVersion.Build);

	public static string UpdateUrl => "https://play.google.com/store/apps/details?id=com.sparklingsociety.cityisland5";

	public static string AppIdentifier => "com.sparklingsociety.cityisland5";

	public static string SocialServiceName => "Google Play";

	public static bool SocialServiceSupported => true;
}
