using System;
using System.Collections.Generic;

namespace CIGMigrator
{
	public class Migrator
	{
		private const string MigrationVersionKey = "_migration_version_";

		private static readonly IMigrator[] ForeverMigrators = new IMigrator[1]
		{
			new ForeverMigrator1()
		};

		private static readonly IMigrator[] GameMigrators = new IMigrator[12]
		{
			new GameMigratorVersion1(),
			new GameMigratorVersion2(),
			new GameMigratorVersion3(),
			new GameMigratorVersion4(),
			new GameMigratorVersion5(),
			new GameMigratorVersion6(),
			new GameMigratorVersion7(),
			new GameMigratorVersion8(),
			new GameMigratorVersion9(),
			new GameMigratorVersion10(),
			new GameMigratorVersion11(),
			new GameMigratorVersion12()
		};

		private static readonly IMigrator[] DeviceMigrators = new IMigrator[1]
		{
			new DeviceMigratorVersion1()
		};

		private readonly Dictionary<string, object> _storage;

		private readonly IMigrator[] _migrators;

		public int CurrentMigrationVersion
		{
			get
			{
				if (_storage.ContainsKey("_migration_version_"))
				{
					if (!(_storage["_migration_version_"] is int) || (int)_storage["_migration_version_"] < 0 || (int)_storage["_migration_version_"] > _migrators.Length)
					{
						throw new Exception(string.Format("{0} Migration failed! MigrationVersion has been corrupted! Expected int between 0 and {1}, Found {2} ", Identifier, _migrators.Length, _storage["_migration_version_"]));
					}
					return (int)_storage["_migration_version_"];
				}
				return 0;
			}
			private set
			{
				_storage["_migration_version_"] = value;
			}
		}

		public string Identifier
		{
			get;
			private set;
		}

		public static int LatestForeverMigrationVersion => ForeverMigrators.Length;

		public static int LatestGameMigrationVersion => GameMigrators.Length;

		private Migrator(string identifier, Dictionary<string, object> storage, IMigrator[] migrators)
		{
			Identifier = identifier;
			_storage = storage;
			_migrators = migrators;
		}

		public void Migrate()
		{
			int currentMigrationVersion = CurrentMigrationVersion;
			int num = _migrators.Length;
			int i;
			for (i = currentMigrationVersion; i < num; i++)
			{
				IMigrator migrator = _migrators[i];
				try
				{
					migrator.Migrate(_storage);
				}
				catch (Exception innerException)
				{
					throw new Exception($"{Identifier} Migrator '{migrator.GetType().Name}' failed! currentVersion={i}->{i + 1}, MigrationVersion={currentMigrationVersion}, Migrators.Count={num}", innerException);
				}
			}
			CurrentMigrationVersion = i;
		}

		public static Migrator CreateForeverMigrator(Dictionary<string, object> storage)
		{
			return new Migrator("Forever", storage, ForeverMigrators);
		}

		public static Migrator CreateGameMigrator(Dictionary<string, object> storage)
		{
			return new Migrator("Game", storage, GameMigrators);
		}

		public static Migrator CreateDeviceMigrator(Dictionary<string, object> storage)
		{
			return new Migrator("Device", storage, DeviceMigrators);
		}
	}
}
