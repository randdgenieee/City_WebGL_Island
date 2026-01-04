namespace CIG
{
	public static class UserIdLogger
	{
		private const string WebServiceStorageKey = "WebService";

		private const string UserIdKey = "UserId";

		private const string HasLoggedKey = "UserIdLogger.HasLogged";

		public static void Log()
		{
			long userId;
			long userId2;
			if (!StorageController.ForeverRoot.Get("UserIdLogger.HasLogged", defaultValue: false) && TryGetUserId(StorageController.ForeverRoot, out userId) && TryGetUserId(StorageController.GameRoot, out userId2) && userId != userId2)
			{
				StorageController.ForeverRoot.Set("UserIdLogger.HasLogged", value: true);
				Analytics.LogFriendcodeChanged(userId, userId2);
			}
		}

		private static bool TryGetUserId(StorageDictionary storageRoot, out long userId)
		{
			if (storageRoot.Contains("WebService"))
			{
				StorageDictionary storageDict = storageRoot.GetStorageDict("WebService");
				if (storageDict.Contains("UserId"))
				{
					userId = storageDict.Get("UserId", -1L);
					return true;
				}
			}
			userId = -1L;
			return false;
		}
	}
}
