namespace CIG
{
	public static class ConflictResolver
	{
		public enum ConflictSolution
		{
			None,
			AskPlayer,
			PickLocal,
			PickCloud,
			InvalidGameVersion,
			SwitchedUser
		}

		private enum CloudGameVersionState
		{
			Newer,
			Older,
			Equal
		}

		public static ConflictSolution Resolve(LocalGameState localState, CloudGameState serverState)
		{
			CloudGameVersionState cloudGameVersionState = ResolveVersion(localState.GameVersion, serverState.GameVersion);
			ConflictSolution result = ConflictSolution.AskPlayer;
			switch (cloudGameVersionState)
			{
			case CloudGameVersionState.Newer:
				result = ConflictSolution.InvalidGameVersion;
				break;
			case CloudGameVersionState.Older:
				result = ResolveMetaData(localState, serverState);
				break;
			case CloudGameVersionState.Equal:
				result = ResolveMetaData(localState, serverState);
				break;
			}
			return result;
		}

		private static CloudGameVersionState ResolveVersion(GameSparksGameVersion localVersion, GameSparksGameVersion remoteVersion)
		{
			if (remoteVersion.Major == localVersion.Major && remoteVersion.Minor == localVersion.Minor)
			{
				return CloudGameVersionState.Equal;
			}
			if (remoteVersion.Major > localVersion.Major || (remoteVersion.Major == localVersion.Major && remoteVersion.Minor > localVersion.Minor))
			{
				return CloudGameVersionState.Newer;
			}
			return CloudGameVersionState.Older;
		}

		private static ConflictSolution ResolveMetaData(LocalGameState localGameState, CloudGameState serverState)
		{
			if (string.IsNullOrEmpty(localGameState.SignedUserId))
			{
				if (!string.IsNullOrEmpty(serverState.SaveGame) || !string.IsNullOrEmpty(serverState.DownloadUrl))
				{
					return ConflictSolution.AskPlayer;
				}
				return ConflictSolution.PickLocal;
			}
			if (localGameState.SignedUserId != serverState.UserId)
			{
				return ConflictSolution.SwitchedUser;
			}
			if (!string.IsNullOrEmpty(serverState.SaveGame) && !string.IsNullOrEmpty(serverState.SaveGuid) && (localGameState.LocalSaveGuid != serverState.SaveGuid || string.IsNullOrEmpty(localGameState.LocalSaveStateGuid)))
			{
				return ConflictSolution.AskPlayer;
			}
			if (serverState.DeviceLastPushedSaveStateGuid == serverState.SaveStateGuid)
			{
				return ConflictSolution.PickLocal;
			}
			bool flag = localGameState.LocalSaveStateGuid == serverState.DeviceLastPushedSaveStateGuid;
			return ConflictSolution.AskPlayer;
		}
	}
}
