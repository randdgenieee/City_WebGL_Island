using System.Collections.Generic;

namespace CIGMigrator
{
	public class GameMigratorVersion7 : IMigrator
	{
		private const string FirstVersionKey = "FirstVersion";

		private const string MajorKey = "Major";

		private const string MinorKey = "Minor";

		private const string RevisionKey = "Revision";

		public void Migrate(Dictionary<string, object> storageRoot)
		{
			if (storageRoot.Count > 0 && !storageRoot.ContainsKey("FirstVersion"))
			{
				Dictionary<string, object> value = new Dictionary<string, object>
				{
					["Major"] = 0,
					["Minor"] = 0,
					["Revision"] = 0
				};
				storageRoot.Add("FirstVersion", value);
			}
		}
	}
}
