using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class LeaderboardsManager
	{
		public delegate void LeaderboardChangedEventHandler(LeaderboardType leaderboardType, Leaderboard leaderboard);

		public delegate void LeaderboardErrorEventHandler(LeaderboardType leaderboardType);

		private class CachedLeaderboard
		{
			private readonly Action<Action<Leaderboard>, Action> _fetchLeaderboard;

			private readonly LeaderboardType _leaderboardType;

			private bool _isUpdating;

			private DateTime _failureCooldownTime;

			public Cache Cache
			{
				get;
				private set;
			}

			public CachedLeaderboard(Cache cache, Action<Action<Leaderboard>, Action> fetchLeaderboard, LeaderboardType leaderboardType)
			{
				Cache = cache;
				_fetchLeaderboard = fetchLeaderboard;
				_leaderboardType = leaderboardType;
			}

			public void GetLeaderboard(Action<LeaderboardType, Leaderboard> onSuccess, Action<LeaderboardType> onError)
			{
				if (!_isUpdating)
				{
					if (Cache.IsValid)
					{
						EventTools.Fire(onSuccess, _leaderboardType, Cache.Leaderboard);
						return;
					}
					if (AntiCheatDateTime.UtcNow < _failureCooldownTime)
					{
						OnError(onSuccess, onError);
						return;
					}
					_isUpdating = true;
					_fetchLeaderboard(delegate(Leaderboard leaderboard)
					{
						_isUpdating = false;
						Cache = new Cache(leaderboard, CacheExpirationTime);
						EventTools.Fire(onSuccess, _leaderboardType, leaderboard);
					}, delegate
					{
						_isUpdating = false;
						_failureCooldownTime = AntiCheatDateTime.UtcNow + FailureExpirationTime;
						OnError(onSuccess, onError);
					});
				}
			}

			private void OnError(Action<LeaderboardType, Leaderboard> onSuccess, Action<LeaderboardType> onError)
			{
				if (Cache.Leaderboard.IsEmpty)
				{
					EventTools.Fire(onError, _leaderboardType);
				}
				else
				{
					EventTools.Fire(onSuccess, _leaderboardType, Cache.Leaderboard);
				}
			}
		}

		private class Cache : IStorable
		{
			private readonly DateTime _expirationDate;

			private const string ExpirationDateKey = "ExpirationDate";

			private const string LeaderboardKey = "Leaderboard";

			public Leaderboard Leaderboard
			{
				get;
			}

			public bool IsValid => AntiCheatDateTime.UtcNow < _expirationDate;

			public Cache()
			{
				Leaderboard = new Leaderboard();
				_expirationDate = DateTime.MinValue;
			}

			public Cache(Leaderboard leaderboard, TimeSpan expiresAfter)
			{
				Leaderboard = leaderboard;
				_expirationDate = AntiCheatDateTime.UtcNow + expiresAfter;
			}

			public Cache(StorageDictionary storage, GameSparksAuthenticator authenticator)
			{
				_expirationDate = storage.GetDateTime("ExpirationDate", AntiCheatDateTime.UtcNow);
				Leaderboard = storage.GetModel("Leaderboard", (StorageDictionary sd) => new Leaderboard(sd, authenticator), new Leaderboard());
			}

			StorageDictionary IStorable.Serialize()
			{
				StorageDictionary storageDictionary = new StorageDictionary();
				storageDictionary.Set("ExpirationDate", _expirationDate);
				storageDictionary.Set("Leaderboard", Leaderboard);
				return storageDictionary;
			}
		}

		public const int MaxLeaderboardItems = 400;

		private const int LeaderboardTopItemCount = 100;

		private const int LeaderboardAroundPadding = 50;

		private const float SyncScoreInterval = 300f;

		private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromMinutes(5.0);

		private static readonly TimeSpan FailureExpirationTime = TimeSpan.FromMinutes(1.0);

		private readonly GameStats _gameStats;

		private readonly GameState _gameState;

		private readonly LikeRegistrar _likeRegistrar;

		private readonly GameSparksAuthenticator _authenticator;

		private readonly GameSparksLeaderboards _gameSparksLeaderboards;

		private readonly string _countryCode;

		private readonly CachedLeaderboard _globalTopLeaderboard;

		private readonly CachedLeaderboard _globalAroundLeaderboard;

		private readonly CachedLeaderboard _countryTopLeaderboard;

		private readonly CachedLeaderboard _countryAroundLeaderboard;

		private const string StorageKey = "LeaderboardsManager";

		private const string GlobalTopCacheKey = "GlobalTopCache";

		private const string GlobalAroundCacheKey = "GlobalAroundCache";

		private const string CountryTopCacheKey = "CountryTop";

		private const string CountryAroundCacheKey = "CountryAround";

		private StorageDictionary _storage;

		private StorageDictionary Storage => _storage ?? (_storage = StorageController.GameRoot.GetStorageDict("LeaderboardsManager"));

		public event LeaderboardChangedEventHandler LeaderboardChangedEvent;

		public event LeaderboardErrorEventHandler LeaderboardErrorEvent;

		private void FireLeaderboardChangedEvent(LeaderboardType leaderboardType, Leaderboard leaderboard)
		{
			this.LeaderboardChangedEvent?.Invoke(leaderboardType, leaderboard);
		}

		private void FireLeaderboardErrorEvent(LeaderboardType leaderboardType)
		{
			this.LeaderboardErrorEvent?.Invoke(leaderboardType);
		}

		public LeaderboardsManager(GameStats gameStats, GameState gameState, GameSparksAuthenticator authenticator, GameSparksLeaderboards gameSparksLeaderboards, LikeRegistrar likeRegistrar, RoutineRunner routineRunner)
		{
			_gameStats = gameStats;
			_gameState = gameState;
			_authenticator = authenticator;
			_gameSparksLeaderboards = gameSparksLeaderboards;
			_likeRegistrar = likeRegistrar;
			_globalTopLeaderboard = new CachedLeaderboard(Storage.GetModel("GlobalTopCache", (StorageDictionary sd) => new Cache(sd, authenticator), new Cache()), FetchGlobalTopLeaderboard, LeaderboardType.GlobalTop);
			_globalAroundLeaderboard = new CachedLeaderboard(Storage.GetModel("GlobalAroundCache", (StorageDictionary sd) => new Cache(sd, authenticator), new Cache()), FetchGlobalAroundLeaderboard, LeaderboardType.GlobalAround);
			_countryTopLeaderboard = new CachedLeaderboard(Storage.GetModel("CountryTop", (StorageDictionary sd) => new Cache(sd, authenticator), new Cache()), FetchCountryTopLeaderboard, LeaderboardType.CountryTop);
			_countryAroundLeaderboard = new CachedLeaderboard(Storage.GetModel("CountryAround", (StorageDictionary sd) => new Cache(sd, authenticator), new Cache()), FetchCountryAroundLeaderboard, LeaderboardType.CountryAround);
			routineRunner.StartCoroutine(SyncRoutine());
		}

		public void GetLeaderboard(LeaderboardType leaderboardType)
		{
			GetCachedLeaderboard(leaderboardType).GetLeaderboard(FireLeaderboardChangedEvent, FireLeaderboardErrorEvent);
		}

		public bool HasCachedLeaderboard(LeaderboardType leaderboardType)
		{
			return !GetCachedLeaderboard(leaderboardType).Cache.Leaderboard.IsEmpty;
		}

		private CachedLeaderboard GetCachedLeaderboard(LeaderboardType leaderboardType)
		{
			switch (leaderboardType)
			{
			case LeaderboardType.CountryAround:
				return _countryAroundLeaderboard;
			case LeaderboardType.CountryTop:
				return _countryTopLeaderboard;
			case LeaderboardType.GlobalAround:
				return _globalAroundLeaderboard;
			case LeaderboardType.GlobalTop:
				return _globalTopLeaderboard;
			default:
				UnityEngine.Debug.LogError($"No leaderboard could be found for type '{leaderboardType}'");
				return _globalTopLeaderboard;
			}
		}

		private void FetchGlobalTopLeaderboard(Action<Leaderboard> onSuccess, Action onError)
		{
			_gameSparksLeaderboards.GetGlobalTop(_likeRegistrar, 100, delegate(Leaderboard leaderboard)
			{
				EventTools.Fire(onSuccess, leaderboard);
			}, delegate(GameSparksException error)
			{
				GameSparksUtils.LogGameSparksError(error);
				EventTools.Fire(onError);
			});
		}

		private void FetchGlobalAroundLeaderboard(Action<Leaderboard> onSuccess, Action onError)
		{
			_gameSparksLeaderboards.GetGlobalAroundUser(_likeRegistrar, 50, delegate(Leaderboard leaderboard)
			{
				EventTools.Fire(onSuccess, leaderboard);
			}, delegate(GameSparksException error)
			{
				GameSparksUtils.LogGameSparksError(error);
				EventTools.Fire(onError);
			});
		}

		private void FetchCountryTopLeaderboard(Action<Leaderboard> onSuccess, Action onError)
		{
			_gameSparksLeaderboards.GetCountryTop(_likeRegistrar, 100, _countryCode, delegate(Leaderboard leaderboard)
			{
				EventTools.Fire(onSuccess, leaderboard);
			}, delegate(GameSparksException error)
			{
				GameSparksUtils.LogGameSparksError(error);
				EventTools.Fire(onError);
			});
		}

		private void FetchCountryAroundLeaderboard(Action<Leaderboard> onSuccess, Action onError)
		{
			_gameSparksLeaderboards.GetCountryAroundUser(_likeRegistrar, 50, _countryCode, delegate(Leaderboard leaderboard)
			{
				EventTools.Fire(onSuccess, leaderboard);
			}, delegate(GameSparksException error)
			{
				GameSparksUtils.LogGameSparksError(error);
				EventTools.Fire(onError);
			});
		}

		private IEnumerator SyncRoutine()
		{
			while (true)
			{
				if (_authenticator.CurrentAuthentication.IsAuthenticated)
				{
					_gameSparksLeaderboards.PostScore(_gameStats.GetIslandScore(), _gameState.GlobalPopulation, _gameState.Level, _countryCode, null, GameSparksUtils.LogGameSparksError);
				}
				yield return new WaitForSecondsRealtime(300f);
			}
		}

		public void Serialize()
		{
			Storage.Set("GlobalTopCache", _globalTopLeaderboard.Cache);
			Storage.Set("GlobalAroundCache", _globalAroundLeaderboard.Cache);
			Storage.Set("CountryTop", _countryTopLeaderboard.Cache);
			Storage.Set("CountryAround", _countryAroundLeaderboard.Cache);
		}
	}
}
