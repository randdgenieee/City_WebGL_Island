using System;

namespace CIG
{
	public class CloudGameState
	{
		public string SaveGame
		{
			get;
			private set;
		}

		public string UserId
		{
			get;
			private set;
		}

		public string SaveGuid
		{
			get;
			private set;
		}

		public string DeviceLastPushedSaveStateGuid
		{
			get;
			private set;
		}

		public string SaveStateGuid
		{
			get;
			private set;
		}

		public DateTime? UTCSaveTime
		{
			get;
			private set;
		}

		public int PlayerLevel
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public GameSparksGameVersion GameVersion
		{
			get;
			private set;
		}

		public string DownloadUrl
		{
			get;
			private set;
		}

		public CloudGameState(string saveGame, string serverUserId, string serverSaveGuid, string deviceLastPushedSaveStateGuid, string serverSaveStateGuid, DateTime? utcSaveTime, int playerLevel, string displayName, GameSparksGameVersion gameVersion, string downloadUrl)
		{
			SaveGame = saveGame;
			UserId = serverUserId;
			SaveGuid = serverSaveGuid;
			DeviceLastPushedSaveStateGuid = deviceLastPushedSaveStateGuid;
			SaveStateGuid = serverSaveStateGuid;
			UTCSaveTime = utcSaveTime;
			PlayerLevel = playerLevel;
			DisplayName = displayName;
			GameVersion = gameVersion;
			DownloadUrl = downloadUrl;
		}
	}
}
