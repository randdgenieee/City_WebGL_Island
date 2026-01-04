using GameSparks.Api.Responses;
using System;

namespace CIG
{
	public class GameSparksLikes
	{
		private readonly CIGGameSparksInstance _gameSparksInstance;

		public GameSparksLikes(CIGGameSparksInstance gameSparksInstance)
		{
			_gameSparksInstance = gameSparksInstance;
		}

		public void AddLike(string userId, Action onSuccess, Action<GameSparksException> onError)
		{
			new IslandVisitingAddLike(_gameSparksInstance, userId).Send(delegate
			{
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("AddLike", errorResponse));
			});
		}

		public void RemoveLike(string userId, Action onSuccess, Action<GameSparksException> onError)
		{
			new IslandVisitingRemoveLike(_gameSparksInstance, userId).Send(delegate
			{
				EventTools.Fire(onSuccess);
			}, delegate(LogEventResponse errorResponse)
			{
				EventTools.Fire(onError, new GameSparksException("RemoveLike", errorResponse));
			});
		}
	}
}
