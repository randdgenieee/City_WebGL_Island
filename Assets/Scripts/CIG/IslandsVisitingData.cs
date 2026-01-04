using GameSparks.Core;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class IslandsVisitingData : IStorable
	{
		public static readonly GameSparksJsonSchema Schema = new GameSparksJsonSchema().Field<GSData>("Islands").Field<int>("Likes").Field<string>("DisplayName")
			.Field<int>("Level")
			.Field<float>("LevelProgress")
			.Field<int>("Happiness")
			.Field<int>("Population")
			.Field<int>("Housing")
			.Field<int>("Employees")
			.Field<int>("Jobs");

		private Dictionary<string, string> _islands;

		private const string UserIdKey = "UserId";

		private const string TimestampKey = "Timestamp";

		private const string LikesKey = "Likes";

		private const string DisplayNameKey = "DisplayName";

		public string UserId
		{
			get;
			private set;
		}

		public DateTime Timestamp
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public float LevelProgress
		{
			get;
			private set;
		}

		public int Happiness
		{
			get;
			private set;
		}

		public int Population
		{
			get;
			private set;
		}

		public int Housing
		{
			get;
			private set;
		}

		public int Employees
		{
			get;
			private set;
		}

		public int Jobs
		{
			get;
			private set;
		}

		private IslandsVisitingData()
		{
		}

		public static IslandsVisitingData CreateFromResponseData(LikeRegistrar likeRegistrar, string userId, GSData islandsData)
		{
			string text = Schema.Validate(islandsData);
			if (!string.IsNullOrEmpty(text))
			{
				throw new GameSparksException("IslandsVisitingDataFromScriptData", text, GSError.SchemaValidationError);
			}
			likeRegistrar.SetLikesForUser(userId, islandsData.GetInt("Likes").Value);
			GSData gSData = islandsData.GetGSData("Islands");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (IslandId allIslandId in IslandExtensions.AllIslandIds)
			{
				string value = gSData.GetString(allIslandId.ToString()) ?? string.Empty;
				dictionary.Add(allIslandId.ToString(), value);
			}
			return new IslandsVisitingData
			{
				UserId = userId,
				_islands = dictionary,
				Timestamp = AntiCheatDateTime.UtcNow,
				DisplayName = islandsData.GetString("DisplayName"),
				Level = islandsData.GetInt("Level").Value,
				LevelProgress = islandsData.GetFloat("LevelProgress").Value,
				Happiness = islandsData.GetInt("Happiness").Value,
				Population = islandsData.GetInt("Population").Value,
				Housing = islandsData.GetInt("Housing").Value,
				Employees = islandsData.GetInt("Employees").Value,
				Jobs = islandsData.GetInt("Jobs").Value
			};
		}

		public bool IsIslandUnlocked(IslandId islandId)
		{
			return !string.IsNullOrEmpty(GetIslandData(islandId));
		}

		public string GetIslandData(IslandId islandId)
		{
			if (!_islands.TryGetValue(islandId.ToString(), out string value))
			{
				return string.Empty;
			}
			return value;
		}

		public IslandsVisitingData(StorageDictionary storage)
		{
			UserId = storage.Get("UserId", string.Empty);
			_islands = storage.GetDictionary<string>("Islands");
			Timestamp = storage.GetDateTime("Timestamp", DateTime.MinValue);
			DisplayName = storage.Get("DisplayName", string.Empty);
			Level = storage.Get("Level", 0);
			LevelProgress = storage.Get("LevelProgress", 0f);
			Happiness = storage.Get("Happiness", 0);
			Population = storage.Get("Population", 0);
			Housing = storage.Get("Housing", 0);
			Employees = storage.Get("Employees", 0);
			Jobs = storage.Get("Jobs", 0);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("UserId", UserId);
			storageDictionary.Set("Islands", _islands);
			storageDictionary.Set("Timestamp", Timestamp);
			storageDictionary.Set("DisplayName", DisplayName);
			storageDictionary.Set("Level", Level);
			storageDictionary.Set("LevelProgress", LevelProgress);
			storageDictionary.Set("Happiness", Happiness);
			storageDictionary.Set("Population", Population);
			storageDictionary.Set("Housing", Housing);
			storageDictionary.Set("Employees", Employees);
			storageDictionary.Set("Jobs", Jobs);
			return storageDictionary;
		}
	}
}
