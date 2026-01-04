using UnityEngine;

namespace CIG
{
	[BalancePropertyClass("walkerBalloon", false)]
	public class WalkerBalloonProperties : BaseProperties
	{
		private const string BalloonTypeKey = "balloonType";

		private const string GroupTypeKey = "groupType";

		private const string ShowDurationSecondsKey = "showDuration";

		private const string CooldownAfterClickMinutesKey = "cooldownAfterClickMinutes";

		private const string CooldownAfterShowMinutesKey = "cooldownAfterShowMinutes";

		[BalanceProperty("balloonType", ParseType = typeof(string))]
		public WalkerBalloonType BalloonType
		{
			get;
		}

		[BalanceProperty("groupType", ParseType = typeof(string))]
		public WalkerBalloonType GroupType
		{
			get;
		}

		[BalanceProperty("showDuration")]
		public double ShowDurationSeconds
		{
			get;
		}

		[BalanceProperty("cooldownAfterClickMinutes")]
		public int CooldownAfterClickMinutes
		{
			get;
		}

		[BalanceProperty("cooldownAfterShowMinutes")]
		public int CooldownAfterShowMinutes
		{
			get;
		}

		public WalkerBalloonProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			string property = GetProperty("balloonType", "unknown");
			if (EnumExtensions.TryParseEnum(property, out WalkerBalloonType parsedEnum))
			{
				BalloonType = parsedEnum;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Cannot parse '{2}.{0}' to {1}", property, typeof(WalkerBalloonType).Name, baseKey);
			}
			string property2 = GetProperty("groupType", "unknown");
			if (EnumExtensions.TryParseEnum(property2, out WalkerBalloonType parsedEnum2))
			{
				GroupType = parsedEnum2;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Cannot parse '{2}.{0}' to {1}", property2, typeof(WalkerBalloonType).Name, baseKey);
			}
			ShowDurationSeconds = GetProperty("showDuration", 30.0);
			CooldownAfterClickMinutes = GetProperty("cooldownAfterClickMinutes", 1);
			CooldownAfterShowMinutes = GetProperty("cooldownAfterShowMinutes", 1);
		}
	}
}
