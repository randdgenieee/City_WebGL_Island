using System.Collections.Generic;

namespace CIGMigrator
{
	public class DeviceMigratorVersion1 : IMigrator
	{
		private const string SettingsKey = "Settings";

		private const string AuthenticationAllowedKey = "AuthenticationAllowed";

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			if (storageRoot.TryGetValue("Settings", out object value) && value is Dictionary<string, object>)
			{
				((Dictionary<string, object>)value)["AuthenticationAllowed"] = true;
			}
		}
	}
}
