namespace CIG
{
	public class LocalGameState
	{
		public string SignedUserId
		{
			get;
			private set;
		}

		public string LocalSaveGuid
		{
			get;
			private set;
		}

		public string LocalSaveStateGuid
		{
			get;
			private set;
		}

		public GameSparksGameVersion GameVersion
		{
			get;
			private set;
		}

		public LocalGameState(GameSparksGameVersion localGameVersion, string localSignedUserId, string localSaveGuid, string localSaveStateGuid)
		{
			GameVersion = localGameVersion;
			SignedUserId = localSignedUserId;
			LocalSaveGuid = localSaveGuid;
			LocalSaveStateGuid = localSaveStateGuid;
		}
	}
}
