using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class LikeRegistrar
	{
		private class UserLikes : IStorable
		{
			private const string UserIdKey = "UserId";

			private const string LikesKey = "Likes";

			public string UserId
			{
				get;
				private set;
			}

			public int Likes
			{
				get;
				set;
			}

			public UserLikes(string userId, int likes)
			{
				UserId = userId;
				Likes = likes;
			}

			public UserLikes(StorageDictionary storage)
			{
				UserId = storage.Get("UserId", string.Empty);
				Likes = storage.Get("Likes", 0);
			}

			public StorageDictionary Serialize()
			{
				StorageDictionary storageDictionary = new StorageDictionary();
				storageDictionary.Set("UserId", UserId);
				storageDictionary.Set("Likes", Likes);
				return storageDictionary;
			}
		}

		private const int VotesAvailableEveryRefresh = 10;

		private readonly StorageDictionary _storage;

		private readonly GameSparksLikes _gameSparksLikes;

		private readonly FriendsManager _friendsManager;

		private readonly HashSet<string> _likedUserIds;

		private readonly List<UserLikes> _userLikeCache;

		private readonly DateTime _refreshTime;

		private const string LikedUserIdsKey = "LikedUserIds";

		private const string AvailableLikesKey = "AvailableLikes";

		private const string RefreshTimeKey = "RefreshTime";

		private const string UserLikeCache = "UserLike";

		public int AvailableLikes
		{
			get;
			private set;
		}

		private int MaxUserLikeCache => 400 + _friendsManager.FriendsCount;

		public LikeRegistrar(StorageDictionary storage, GameSparksLikes gameSparksLikes, FriendsManager friendsManager)
		{
			_storage = storage;
			_gameSparksLikes = gameSparksLikes;
			_friendsManager = friendsManager;
			_likedUserIds = _storage.GetHashSet<string>("LikedUserIds");
			AvailableLikes = _storage.Get("AvailableLikes", 10);
			_refreshTime = _storage.GetDateTime("RefreshTime", DateTime.MinValue);
			_userLikeCache = _storage.GetModels("UserLike", (StorageDictionary sd) => new UserLikes(sd));
			if (_refreshTime < AntiCheatDateTime.Now)
			{
				AvailableLikes = 10;
				_refreshTime = AntiCheatDateTime.Today.AddDays(1.0);
			}
			UpdateUserLikeCache();
		}

		public void UserAddedLike(string userId, Action onSuccess, Action onError)
		{
			_gameSparksLikes.AddLike(userId, delegate
			{
				AddLikeSuccess(userId);
				EventTools.Fire(onSuccess);
			}, delegate(GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
				EventTools.Fire(onError);
			});
		}

		public void UserRemovedLike(string userId, Action onSuccess, Action onError)
		{
			_gameSparksLikes.RemoveLike(userId, delegate
			{
				RemoveLikeSuccess(userId);
				EventTools.Fire(onSuccess);
			}, delegate(GameSparksException exception)
			{
				GameSparksUtils.LogGameSparksError(exception);
				EventTools.Fire(onError);
			});
		}

		public bool HasLikedUser(string userId)
		{
			return _likedUserIds.Contains(userId);
		}

		public int GetLikesForUser(string userId)
		{
			return _userLikeCache.Find((UserLikes userLikes) => userLikes.UserId == userId)?.Likes ?? 0;
		}

		public void SetLikesForUser(string userId, int likes)
		{
			UserLikes userLikes2 = _userLikeCache.Find((UserLikes userLikes) => userLikes.UserId == userId);
			if (userLikes2 != null)
			{
				userLikes2.Likes = likes;
				return;
			}
			_userLikeCache.Add(new UserLikes(userId, likes));
			UpdateUserLikeCache();
		}

		private void AddLikeSuccess(string userId)
		{
			if (AvailableLikes > 0)
			{
				_likedUserIds.Add(userId);
				AvailableLikes--;
				UpdateLikesForUser(userId, 1);
			}
		}

		private void RemoveLikeSuccess(string userId)
		{
			if (_likedUserIds.Remove(userId))
			{
				AvailableLikes++;
				UpdateLikesForUser(userId, -1);
			}
		}

		private void UpdateUserLikeCache()
		{
			int count = _userLikeCache.Count;
			if (count > MaxUserLikeCache)
			{
				_userLikeCache.RemoveRange(0, count - MaxUserLikeCache);
			}
		}

		private void UpdateLikesForUser(string userId, int delta)
		{
			UserLikes userLikes2 = _userLikeCache.Find((UserLikes userLikes) => userLikes.UserId == userId);
			if (userLikes2 != null)
			{
				userLikes2.Likes += delta;
				return;
			}
			_userLikeCache.Add(new UserLikes(userId, Mathf.Max(delta, 0)));
			UpdateUserLikeCache();
		}

		public void Serialize()
		{
			_storage.Set("LikedUserIds", _likedUserIds);
			_storage.Set("AvailableLikes", AvailableLikes);
			_storage.Set("RefreshTime", _refreshTime);
			_storage.Set("UserLike", _userLikeCache);
		}
	}
}
