using System;

namespace CIG
{
	public abstract class DailyManager
	{
		protected readonly StorageDictionary _storage;

		private const string EndTimeStampTicksKey = "endTimeStampTicks";

		public TimeSpan TimeLeft
		{
			get
			{
				if (EndTimestampUTC.HasValue)
				{
					return EndTimestampUTC.Value - AntiCheatDateTime.UtcNow;
				}
				return TimeSpan.Zero;
			}
		}

		protected DateTime? EndTimestampUTC
		{
			get;
			private set;
		}

		private bool HasDayLapsed
		{
			get
			{
				if (EndTimestampUTC.HasValue)
				{
					return EndTimestampUTC.Value < AntiCheatDateTime.UtcNow;
				}
				return true;
			}
		}

		protected DailyManager(StorageDictionary storage)
		{
			_storage = storage;
			if (_storage.Contains("endTimeStampTicks"))
			{
				long ticks = _storage.Get("endTimeStampTicks", 0L);
				EndTimestampUTC = new DateTime(ticks);
			}
			else
			{
				EndTimestampUTC = null;
			}
		}

		protected void TryLapseDay()
		{
			if (HasDayLapsed)
			{
				TimeSpan timeSinceLapse = EndTimestampUTC.HasValue ? (AntiCheatDateTime.UtcNow - EndTimestampUTC.Value) : TimeSpan.Zero;
				EndTimestampUTC = AntiCheatDateTime.TimeTillMidnight;
				OnDayLapsed(timeSinceLapse);
			}
		}

		protected abstract void OnDayLapsed(TimeSpan timeSinceLapse);

		public virtual void Serialize()
		{
			if (EndTimestampUTC.HasValue)
			{
				_storage.Set("endTimeStampTicks", EndTimestampUTC.Value.Ticks);
			}
			else
			{
				_storage.Remove("endTimeStampTicks");
			}
		}
	}
}
