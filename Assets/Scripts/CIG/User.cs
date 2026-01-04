namespace CIG
{
	public class User
	{
		private const string UserKeyKey = "UserKey";

		private const string IsPayingUserKey = "IsPayingUser";

		private readonly StorageDictionary _storage;

		public string UserKey
		{
			get;
			private set;
		}

		public bool IsPayingUser
		{
			get;
			private set;
		}

		public User(StorageDictionary storage)
		{
			_storage = storage;
			UserKey = _storage.Get("UserKey", string.Empty);
			IsPayingUser = _storage.Get("IsPayingUser", defaultValue: false);
		}

		public void SetUserKey(string userKey)
		{
			if (UserKey != userKey)
			{
				UserKey = userKey;
				Analytics.SetFriendCode(UserKey);
			}
		}

		public void SetPayingUser()
		{
			IsPayingUser = true;
		}

		public void Serialize()
		{
			_storage.Set("UserKey", UserKey);
			_storage.Set("IsPayingUser", IsPayingUser);
		}
	}
}
