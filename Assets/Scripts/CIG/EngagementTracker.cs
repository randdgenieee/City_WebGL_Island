using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public sealed class EngagementTracker
	{
		private const float EventWaitTimeSeconds = 60f;

		private DateTime _lastEventSentTime;

		public EngagementTracker(RoutineRunner routineRunner)
		{
			ResetLastEventSentTime();
			routineRunner.StartCoroutine(EngagementEventRoutine());
		}

		public void ApplicationPause(bool pause)
		{
			if (pause)
			{
				SendEngagementEvent();
				Analytics.LogEvent("app_to_background");
			}
			else
			{
				ResetLastEventSentTime();
				Analytics.LogEvent("app_to_foreground");
			}
		}

		public void ApplicationQuit()
		{
			SendEngagementEvent();
		}

		private void ResetLastEventSentTime()
		{
			_lastEventSentTime = AntiCheatDateTime.UtcNow;
		}

		private void SendEngagementEvent()
		{
			Analytics.Engagement((long)(AntiCheatDateTime.UtcNow - _lastEventSentTime).TotalMilliseconds);
			ResetLastEventSentTime();
		}

		private IEnumerator EngagementEventRoutine()
		{
			while (true)
			{
				SendEngagementEvent();
				yield return new WaitForSecondsRealtime(60f);
			}
		}
	}
}
