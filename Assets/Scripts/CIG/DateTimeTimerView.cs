using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class DateTimeTimerView : TimerView
	{
		[SerializeField]
		protected LocalizedText _timerLabel;

		private DateTime _endDateTime;

		protected override float TimeLeftSeconds => (float)(_endDateTime - AntiCheatDateTime.UtcNow).TotalSeconds;

		public void StartTimer(DateTime endDateTime)
		{
			_endDateTime = endDateTime;
			StartTimer();
		}

		public override void StopTimer()
		{
			_endDateTime = DateTime.MinValue;
			base.StopTimer();
		}

		protected override void UpdateState()
		{
			base.gameObject.SetActive(_isActive);
			base.UpdateState();
		}

		protected override void UpdateTime(float seconds)
		{
			_timerLabel.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(seconds), hideSecondPartWhenZero: true);
		}
	}
}
