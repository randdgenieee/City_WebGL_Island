using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class GameSparksLeaderboards
	{
		private const string ItemsKey = "items";

		private static readonly GameSparksJsonSchema GetResponseSchema = new GameSparksJsonSchema().Field<List<GSData>>("items");

		private readonly CIGGameSparksInstance _instance;

		private readonly GameSparksAuthenticator _authenticator;

		public GameSparksLeaderboards(CIGGameSparksInstance instance, GameSparksAuthenticator authenticator)
		{
			_instance = instance;
			_authenticator = authenticator;
		}

		public void GetGlobalTop(LikeRegistrar likeRegistrar, int maxEntries, Action<Leaderboard> onSuccess, Action<GameSparksException> onError)
		{
			new GetGlobalLeaderboardTop(_instance, maxEntries).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					Leaderboard value = new Leaderboard(ParseGetResponse(successResponse, likeRegistrar));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("GetGlobalLeaderboardTop", errorResponse));
			});
		}

		public void GetGlobalAroundUser(LikeRegistrar likeRegistrar, int padding, Action<Leaderboard> onSuccess, Action<GameSparksException> onError)
		{
			new GetGlobalLeaderboardAroundUser(_instance, padding).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					Leaderboard value = new Leaderboard(ParseGetResponse(successResponse, likeRegistrar));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("GetGlobalLeaderboardAroundUser", errorResponse));
			});
		}

		public void GetCountryTop(LikeRegistrar likeRegistrar, int maxEntries, string country, Action<Leaderboard> onSuccess, Action<GameSparksException> onError)
		{
			new GetCountryLeaderboardTop(_instance, maxEntries, country).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					Leaderboard value = new Leaderboard(ParseGetResponse(successResponse, likeRegistrar));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("GetCountryLeaderboardTop", errorResponse));
			});
		}

		public void GetCountryAroundUser(LikeRegistrar likeRegistrar, int padding, string country, Action<Leaderboard> onSuccess, Action<GameSparksException> onError)
		{
			new GetCountryLeaderboardAroundUser(_instance, padding, country).Send(delegate(LogEventResponse successResponse)
			{
				try
				{
					Leaderboard value = new Leaderboard(ParseGetResponse(successResponse, likeRegistrar));
					EventTools.Fire(onSuccess, value);
				}
				catch (GameSparksException value2)
				{
					EventTools.Fire(onError, value2);
				}
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("GetCountryLeaderboardAroundUser", errorResponse));
			});
		}

		public void PostScore(int score, int population, int level, string country, Action onSuccess, Action<GameSparksException> onError)
		{
			new PostLeaderboardScore(_instance, score, population, level, country).Send(delegate
			{
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("PostLeaderboardScore", errorResponse));
			});
		}

		private List<LeaderboardEntry> ParseGetResponse(GSTypedResponse response, LikeRegistrar likeRegistrar)
		{
			string text = GetResponseSchema.Validate(response.ScriptData);
			if (!string.IsNullOrEmpty(text))
			{
				throw new GameSparksException("ParseGetResponse", text, GSError.SchemaValidationError);
			}
			List<GSData> gSDataList = response.ScriptData.GetGSDataList("items");
			List<LeaderboardEntry> list = new List<LeaderboardEntry>();
			int i = 0;
			for (int count = gSDataList.Count; i < count; i++)
			{
				LeaderboardEntry item = new LeaderboardEntry(gSDataList[i], _authenticator, likeRegistrar);
				list.Add(item);
			}
			return list;
		}
	}
}
