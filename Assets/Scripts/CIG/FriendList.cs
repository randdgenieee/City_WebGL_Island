using SparkLinq;
using System.Collections.Generic;

namespace CIG
{
	public class FriendList : IStorable
	{
		public delegate void FriendRemovedEventHandler(Friend friend);

		private const string FriendDatasKey = "FriendDatas";

		public List<Friend> Friends
		{
			get;
		}

		public bool IsEmpty => Friends.Count == 0;

		public event FriendRemovedEventHandler FriendRemovedEvent;

		private void FireFriendRemovedEvent(Friend friend)
		{
			this.FriendRemovedEvent?.Invoke(friend);
		}

		public FriendList()
		{
			Friends = new List<Friend>();
		}

		public FriendList(List<FriendData> friendDatas)
		{
			Friends = ListExtensions.ConvertAll(friendDatas, (FriendData d) => new Friend(d));
			SortFriends();
		}

		public void UpdateFriendEntry(FriendData friendData, bool add)
		{
			int i = 0;
			for (int count = Friends.Count; i < count; i++)
			{
				if (Friends[i].FriendCode == friendData.FriendCode)
				{
					Friends[i].UpdateData(friendData);
					return;
				}
			}
			if (add)
			{
				Friends.Add(new Friend(friendData));
				SortFriends();
			}
		}

		public void RemoveFriendEntry(string friendCode)
		{
			int num = Friends.FindIndex((Friend f) => f.FriendCode == friendCode);
			if (num >= 0 && num < Friends.Count)
			{
				Friend friend = Friends[num];
				Friends.RemoveAt(num);
				FireFriendRemovedEvent(friend);
			}
		}

		public bool HasFriend(string userId)
		{
			return Friends.Any((Friend f) => f.UserId == userId);
		}

		private void SortFriends()
		{
			Friends.Sort(delegate(Friend lhs, Friend rhs)
			{
				int num = lhs.FriendStatus.CompareTo(rhs.FriendStatus);
				if (num == 0)
				{
					num = lhs.DisplayName.CompareTo(rhs.DisplayName);
				}
				return num;
			});
		}

		public FriendList(StorageDictionary storage)
		{
			Friends = storage.GetModels("FriendDatas", (StorageDictionary sd) => new Friend(sd));
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("FriendDatas", Friends);
			return storageDictionary;
		}
	}
}
