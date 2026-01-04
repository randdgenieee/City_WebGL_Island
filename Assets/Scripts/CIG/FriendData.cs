using GameSparks.Core;

namespace CIG
{
	public class FriendData
	{
		private const string UserIdKey = "UserId";

		private const string DisplayNameKey = "DisplayName";

		private const string FriendCodeKey = "FriendCode";

		private const string ScoreKey = "LeaderboardScore";

		private const string LevelKey = "PlayerLevel";

		private const string StatusKey = "Status";

		private const string CanVisitKey = "CanVisit";

		private const string CanVisitWithUploadablesKey = "CanVisitWithUploadables";

		private const string CanReceiveGiftKey = "CanReceiveGift";

		private const string CanSendGiftKey = "CanSendGift";

		private const string RemainingCooldownKey = "RemainingCooldown";

		private static readonly GameSparksJsonSchema Schema = new GameSparksJsonSchema().Field<string>("UserId").Field<string>("DisplayName").Field<string>("FriendCode")
			.Field<int>("PlayerLevel")
			.Field<int>("LeaderboardScore")
			.Field<string>("Status")
			.Field<bool>("CanVisit")
			.Field<bool>("CanVisitWithUploadables")
			.Field<bool>("CanReceiveGift")
			.Field<bool>("CanSendGift")
			.Field<double>("RemainingCooldown");

		public string UserId
		{
			get;
		}

		public string DisplayName
		{
			get;
		}

		public string FriendCode
		{
			get;
		}

		public int Level
		{
			get;
		}

		public int Score
		{
			get;
		}

		public FriendStatusType FriendStatus
		{
			get;
		}

		public bool CanVisit
		{
			get;
		}

		public bool CanReceiveGift
		{
			get;
		}

		public bool CanSendGift
		{
			get;
		}

		public double RemainingCooldown
		{
			get;
		}

		public FriendData(GSData data)
		{
			string text = Schema.Validate(data);
			if (!string.IsNullOrEmpty(text))
			{
				throw new GameSparksException("FriendData Constructor", text, GSError.SchemaValidationError);
			}
			UserId = data.GetString("UserId");
			DisplayName = data.GetString("DisplayName");
			FriendCode = data.GetString("FriendCode");
			Level = (data.GetInt("PlayerLevel") ?? 0);
			Score = (data.GetInt("LeaderboardScore") ?? 0);
			FriendStatus = ParseFriendStatus(data.GetString("Status"));
			CanVisit = (data.GetBoolean("CanVisit").Value || data.GetBoolean("CanVisitWithUploadables").Value);
			CanReceiveGift = (data.GetBoolean("CanReceiveGift") ?? false);
			CanSendGift = (data.GetBoolean("CanSendGift") ?? false);
			RemainingCooldown = (data.GetDouble("RemainingCooldown") ?? (-1.0));
		}

		public override string ToString()
		{
			return $"{DisplayName}, userId: {UserId}, friendCode: {FriendCode}, level: {Level}, score: {Score}, status: {FriendStatus}, canVisit: {CanVisit}";
		}

		private FriendStatusType ParseFriendStatus(string status)
		{
			if (!(status == "Accepted"))
			{
				if (!(status == "Sent"))
				{
					if (!(status == "Received"))
					{
						if (!(status == "Declined"))
						{
							if (status == "Suggested")
							{
								return FriendStatusType.Suggested;
							}
							return FriendStatusType.Unknown;
						}
						return FriendStatusType.Declined;
					}
					return FriendStatusType.Received;
				}
				return FriendStatusType.Sent;
			}
			return FriendStatusType.Accepted;
		}
	}
}
