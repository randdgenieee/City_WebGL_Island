using GameSparks.Core;

namespace CIG
{
	public class LeaderboardEntry : IStorable
	{
		private const string ScoreKey = "Score";

		private const string PopulationKey = "Population";

		private const string LevelKey = "Level";

		private const string RankKey = "Rank";

		private const string UserIdKey = "UserId";

		private const string DisplayNameKey = "DisplayName";

		private const string LikesKey = "Likes";

		private const string CanVisitKey = "CanVisit";

		private const string CanVisitWithUploadablesKey = "CanVisitWithUploadables";

		private static readonly GameSparksJsonSchema Schema = new GameSparksJsonSchema().Field<int>("Score").Field<string>("UserId").Field<int>("Rank")
			.Field<string>("DisplayName")
			.Field<int>("Population")
			.Field<int>("Level")
			.Field<int>("Likes")
			.Field<bool>("CanVisit")
			.Field<bool>("CanVisitWithUploadables");

		private readonly GameSparksAuthenticator _authenticator;

		private bool _canVisit;

		public int Score
		{
			get;
			private set;
		}

		public int Population
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public int Rank
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public bool CanVisit
		{
			get
			{
				if (_canVisit)
				{
					return _authenticator.CurrentAuthentication.UserId != UserId;
				}
				return false;
			}
			private set
			{
				_canVisit = value;
			}
		}

		public LeaderboardEntry(GSData data, GameSparksAuthenticator authenticator, LikeRegistrar likeRegistrar)
		{
			_authenticator = authenticator;
			string text = Schema.Validate(data);
			if (!string.IsNullOrEmpty(text))
			{
				throw new GameSparksException("LeaderboardEntry Constructor", text, GSError.SchemaValidationError);
			}
			Score = (data.GetInt("Score") ?? 0);
			Population = (data.GetInt("Population") ?? 0);
			Level = (data.GetInt("Level") ?? 0);
			Rank = (data.GetInt("Rank") ?? 0);
			UserId = data.GetString("UserId");
			DisplayName = data.GetString("DisplayName");
			_canVisit = (data.GetBoolean("CanVisit").Value || data.GetBoolean("CanVisitWithUploadables").Value);
			likeRegistrar.SetLikesForUser(UserId, data.GetInt("Likes").Value);
		}

		public LeaderboardEntry(GameSparksAuthenticator authenticator, int score, int level, int population, int rank, bool canVisit)
		{
			_authenticator = authenticator;
			UserId = _authenticator.CurrentAuthentication.UserId;
			DisplayName = _authenticator.CurrentAuthentication.DisplayName;
			Score = score;
			Level = level;
			Population = population;
			Rank = rank;
			CanVisit = canVisit;
		}

		public LeaderboardEntry(StorageDictionary storage, GameSparksAuthenticator authenticator)
		{
			_authenticator = authenticator;
			Score = storage.Get("Score", 0);
			Population = storage.Get("Population", 0);
			Level = storage.Get("Level", 0);
			Rank = storage.Get("Rank", 0);
			UserId = storage.Get("UserId", string.Empty);
			DisplayName = storage.Get("DisplayName", string.Empty);
			_canVisit = storage.Get("CanVisit", defaultValue: false);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Score", Score);
			storageDictionary.Set("Population", Population);
			storageDictionary.Set("Level", Level);
			storageDictionary.Set("Rank", Rank);
			storageDictionary.Set("UserId", UserId);
			storageDictionary.Set("DisplayName", DisplayName);
			storageDictionary.Set("CanVisit", _canVisit);
			return storageDictionary;
		}
	}
}
