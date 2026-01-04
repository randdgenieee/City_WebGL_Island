using UnityEngine;

namespace CIG
{
	public class UpspeedableProcess : CustomYieldInstruction, IStorable
	{
		public delegate void UpspeededEventHandler();

		private readonly Timing _timing;

		private readonly Multipliers _multipliers;

		private readonly GameState _gameState;

		private readonly CurrenciesSpentReason _speedupCurrenciesSpentReason;

		private const string StartTimeKey = "StartTime";

		private const string EndTimeKey = "EndTime";

		private const string SpeeduppedKey = "Speedupped";

		private const string CancelledKey = "Cancelled";

		private const string SpeedupCurrenciesSpentReasonKey = "SpeedupCurrenciesSpentReason";

		public double EndTime
		{
			get;
			private set;
		}

		public double StartTime
		{
			get;
			private set;
		}

		public double Duration => EndTime - StartTime;

		public double TimeLeft => EndTime - _timing.GameTime;

		public float CompletionPercentage => 1f - Mathf.Clamp01((float)(TimeLeft / Duration));

		public Currency UpspeedCost => Currency.GoldCurrency(CIGUtilities.Round(GoldCostUtility.GetSpeedupCostGoldForSeconds(_multipliers, (long)TimeLeft), RoundingMethod.Nearest, 0));

		public bool Cancelled
		{
			get;
			private set;
		}

		public bool Speedupped
		{
			get;
			private set;
		}

		public bool CanSpeedup => keepWaiting;

		public override bool keepWaiting
		{
			get
			{
				if (Cancelled || Speedupped)
				{
					return false;
				}
				return TimeLeft > 0.0;
			}
		}

		public event UpspeededEventHandler UpspeededEvent;

		public void FireUpspeededEvent()
		{
			this.UpspeededEvent?.Invoke();
		}

		public UpspeedableProcess(Timing timing, Multipliers multipliers, GameState gameState, double duration, CurrenciesSpentReason speedupCurrenciesSpentReason)
		{
			_timing = timing;
			_multipliers = multipliers;
			_gameState = gameState;
			StartTime = _timing.GameTime;
			EndTime = StartTime + duration;
			_speedupCurrenciesSpentReason = speedupCurrenciesSpentReason;
		}

		public UpspeedableProcess(StorageDictionary storage, Timing timing, Multipliers multipliers, GameState gameState)
		{
			_timing = timing;
			_multipliers = multipliers;
			_gameState = gameState;
			StartTime = storage.Get("StartTime", 0.0);
			EndTime = storage.Get("EndTime", 0.0);
			Speedupped = storage.Get("Speedupped", defaultValue: false);
			Cancelled = storage.Get("Cancelled", defaultValue: false);
			_speedupCurrenciesSpentReason = (CurrenciesSpentReason)storage.Get("SpeedupCurrenciesSpentReason", 0);
		}

		public bool PaidSpeedup()
		{
			if (_gameState.SpendCurrencies(UpspeedCost, _speedupCurrenciesSpentReason, null))
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Speedup);
				Cancelled = false;
				Speedupped = true;
				EndTime = _timing.GameTime;
				FireUpspeededEvent();
				return true;
			}
			return false;
		}

		public void FreeSpeedup()
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Speedup);
			Cancelled = false;
			Speedupped = true;
		}

		public void OverrideDuration(double duration)
		{
			StartTime = _timing.GameTime;
			EndTime = _timing.GameTime + duration;
		}

		public void Cancel()
		{
			Cancelled = true;
			Speedupped = false;
		}

		public virtual StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("StartTime", StartTime);
			storageDictionary.Set("EndTime", EndTime);
			storageDictionary.Set("Speedupped", Speedupped);
			storageDictionary.Set("Cancelled", Cancelled);
			storageDictionary.Set("SpeedupCurrenciesSpentReason", (int)_speedupCurrenciesSpentReason);
			return storageDictionary;
		}
	}
}
