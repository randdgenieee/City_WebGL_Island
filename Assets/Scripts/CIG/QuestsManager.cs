using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class QuestsManager : DailyManager
	{
		private const string OngoingQuestsManagerKey = "OngoingQuestsManager";

		private const string DailyQuestsManagerKey = "DailyQuestsManager";

		public OngoingQuestsManager OngoingQuestsManager
		{
			get;
		}

		public DailyQuestsManager DailyQuestsManager
		{
			get;
		}

		public QuestsManager(StorageDictionary storage, GameState gameState, GameStats gameStats, RoutineRunner routineRunner, QuestFactory questFactory, QuestsManagerProperties properties)
			: base(storage)
		{
			OngoingQuestsManager = _storage.Get("OngoingQuestsManager", null, (StorageDictionary sd) => new OngoingQuestsManager(sd, properties.OngoingQuestsManagerProperties, questFactory, gameState, gameStats));
			DailyQuestsManager = _storage.Get("DailyQuestsManager", null, (StorageDictionary sd) => new DailyQuestsManager(sd, properties.DailyQuestsManagerProperties, questFactory, gameState, gameStats));
			TryLapseDay();
			routineRunner.StartCoroutine(LapseDayRoutine());
		}

		protected override void OnDayLapsed(TimeSpan timeSinceLapse)
		{
			DailyQuestsManager.OnDayLapsed();
		}

		private IEnumerator LapseDayRoutine()
		{
			while (true)
			{
				yield return new WaitUntil(() => base.EndTimestampUTC.HasValue);
				yield return new WaitUntilUTCDateTime(base.EndTimestampUTC.Value);
				TryLapseDay();
			}
		}

		public override void Serialize()
		{
			base.Serialize();
			_storage.Set("OngoingQuestsManager", OngoingQuestsManager.Serialize());
			_storage.Set("DailyQuestsManager", DailyQuestsManager.Serialize());
		}
	}
}
