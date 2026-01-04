using System.Collections.Generic;

namespace CIGMigrator
{
	public interface IMigrator
	{
		void Migrate(Dictionary<string, object> storageRoot);
	}
}
