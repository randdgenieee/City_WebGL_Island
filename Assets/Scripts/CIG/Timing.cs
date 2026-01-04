using System;
using UnityEngine;

namespace CIG
{
	public class Timing
	{
		private class TimeGroup : IStorable
		{
			private const string BaseTimeKey = "BaseTime";

			private const string StartTicksKey = "StartTicks";

			private const string ScaleKey = "Scale";

			public long BaseTime
			{
				get;
				set;
			}

			public long StartTicks
			{
				get;
				set;
			}

			public double Scale
			{
				get;
				set;
			}

			public static TimeGroup CreateNew()
			{
				return new TimeGroup
				{
					BaseTime = 0L,
					StartTicks = AntiCheatDateTime.UtcNow.Ticks,
					Scale = 1.0
				};
			}

			private TimeGroup()
			{
			}

			public TimeGroup(StorageDictionary storage)
			{
				BaseTime = storage.Get("BaseTime", 0L);
				StartTicks = storage.Get("StartTicks", AntiCheatDateTime.UtcNow.Ticks);
				Scale = storage.Get("Scale", 1.0);
			}

			StorageDictionary IStorable.Serialize()
			{
				StorageDictionary storageDictionary = new StorageDictionary();
				storageDictionary.Set("BaseTime", BaseTime);
				storageDictionary.Set("StartTicks", StartTicks);
				storageDictionary.Set("Scale", Scale);
				return storageDictionary;
			}
		}

		private const string UnscaledGroupKey = "UnscaledGroup";

		private const string GameGroupKey = "GameGroup";

		private const string AnimationGroupKey = "AnimationGroup";

		private readonly StorageDictionary _storage;

		public double GameTime => GetTime(DeltaTimeType.Game);

		public double AnimationTime => GetTime(DeltaTimeType.Animation);

		public TimeSpan AnimationTimeSpan => TimeSpan.FromSeconds(GetTime(DeltaTimeType.Animation));

		public static double UtcNow => (double)AntiCheatDateTime.UtcNow.Ticks * 1E-07;

		public Timing(StorageDictionary storage)
		{
			_storage = storage;
		}

		public double GetTimeScale(DeltaTimeType deltaTimeType)
		{
			return GetTimeGroup(deltaTimeType).Scale;
		}

		public void SetTimeScale(DeltaTimeType deltaTimeType, double scale)
		{
			if (deltaTimeType == DeltaTimeType.Unscaled)
			{
				UnityEngine.Debug.LogError("Cannot change the timescale of the Unscaled time group.");
				return;
			}
			scale = Math.Max(1.0, scale);
			TimeGroup timeGroup = GetTimeGroup(deltaTimeType);
			timeGroup.BaseTime = (long)(10000000.0 * GetTime(deltaTimeType));
			timeGroup.StartTicks = AntiCheatDateTime.UtcNow.Ticks;
			timeGroup.Scale = scale;
			SaveTimeGroup(deltaTimeType, timeGroup);
		}

		public float GetDeltaTime(DeltaTimeType deltaTimeType)
		{
			return Time.unscaledDeltaTime * (float)GetTimeScale(deltaTimeType);
		}

		public long ConvertYMDhmsToUtc(string ymdhms)
		{
			try
			{
				int year = int.Parse(ymdhms.Substring(0, 4));
				int month = int.Parse(ymdhms.Substring(4, 2));
				int day = int.Parse(ymdhms.Substring(6, 2));
				int hour = int.Parse(ymdhms.Substring(8, 2));
				int minute = int.Parse(ymdhms.Substring(10, 2));
				int second = int.Parse(ymdhms.Substring(12, 2));
				return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local).ToUniversalTime().Ticks / 10000000;
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogWarning($"Unable to parse ymdhms format {ymdhms}: {arg}.");
				return 0L;
			}
		}

		private string GetTimeGroupKey(DeltaTimeType deltaTimeType)
		{
			switch (deltaTimeType)
			{
			case DeltaTimeType.Animation:
				return "AnimationGroup";
			case DeltaTimeType.Game:
				return "GameGroup";
			default:
				return "UnscaledGroup";
			}
		}

		private TimeGroup GetTimeGroup(DeltaTimeType deltaTimeType)
		{
			string timeGroupKey = GetTimeGroupKey(deltaTimeType);
			if (!_storage.Contains(timeGroupKey))
			{
				SaveTimeGroup(deltaTimeType, TimeGroup.CreateNew());
			}
			if (_storage.Contains(timeGroupKey))
			{
				return new TimeGroup(_storage.GetStorageDict(timeGroupKey));
			}
			return null;
		}

		private double GetTime(DeltaTimeType deltaTimeType)
		{
			TimeGroup timeGroup = GetTimeGroup(deltaTimeType);
			return ((double)timeGroup.BaseTime + (double)(AntiCheatDateTime.UtcNow.Ticks - timeGroup.StartTicks) * timeGroup.Scale) * 1E-07;
		}

		private long GetBaseTime(DeltaTimeType deltaTimeType)
		{
			return GetTimeGroup(deltaTimeType).BaseTime;
		}

		private long GetStartTicks(DeltaTimeType deltaTimeType)
		{
			return GetTimeGroup(deltaTimeType).StartTicks;
		}

		private void SaveTimeGroup(DeltaTimeType deltaTimeType, TimeGroup timeGroup)
		{
			_storage.Set(GetTimeGroupKey(deltaTimeType), timeGroup);
		}
	}
}
