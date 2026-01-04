using UnityEngine;

namespace CIGMigrator
{
	public class GameMigratorVersion2Parameters : ScriptableObject
	{
		[SerializeField]
		private IslandSetup[] _islandSetups;

		public IslandSetup[] IslandSetups => _islandSetups;
	}
}
