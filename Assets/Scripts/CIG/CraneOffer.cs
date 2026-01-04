using System;
using System.Collections;

namespace CIG
{
	public abstract class CraneOffer : IStorable
	{
		public enum CraneOfferType
		{
			IAP,
			Currency
		}

		private readonly RoutineRunner _routineRunner;

		private readonly IAPStore<TOCIStoreProduct> _storeManager;

		private readonly GameState _gameState;

		private readonly Action<CraneOffer> _onStart;

		private readonly Action _onEnd;

		private IEnumerator _offerRoutine;

		private const string StartDateTimeKey = "StartDateTime";

		private const string EndDateTimeKey = "EndDateTime";

		public DateTime StartDateTime
		{
			get;
		}

		public DateTime EndDateTime
		{
			get;
		}

		public bool IsActive
		{
			get
			{
				if (AntiCheatDateTime.UtcNow > StartDateTime)
				{
					return AntiCheatDateTime.UtcNow < EndDateTime;
				}
				return false;
			}
		}

		public TimeSpan TimeRemaining => EndDateTime - AntiCheatDateTime.UtcNow;

		public abstract int Cranes
		{
			get;
		}

		protected CraneOffer(RoutineRunner routineRunner, Action<CraneOffer> onStart, Action onEnd, StorageDictionary storage)
			: this(routineRunner, onStart, onEnd, storage.GetDateTime("StartDateTime", DateTime.MinValue), storage.GetDateTime("EndDateTime", DateTime.MinValue))
		{
		}

		protected CraneOffer(RoutineRunner routineRunner, Action<CraneOffer> onStart, Action onEnd, DateTime startDateTime, DateTime endDateTime)
		{
			_routineRunner = routineRunner;
			_onStart = onStart;
			_onEnd = onEnd;
			StartDateTime = startDateTime;
			EndDateTime = endDateTime;
		}

		public virtual void StartRoutine()
		{
			if (_offerRoutine == null)
			{
				_routineRunner.StartCoroutine(_offerRoutine = OfferRoutine());
			}
		}

		public void EndOffer()
		{
			if (_offerRoutine != null)
			{
				_routineRunner.StopCoroutine(_offerRoutine);
				_offerRoutine = null;
			}
			EventTools.Fire(_onEnd);
		}

		private IEnumerator OfferRoutine()
		{
			yield return new WaitUntilUTCDateTime(StartDateTime);
			EventTools.Fire(_onStart, this);
			Analytics.LogEvent("crane_offer_started");
			yield return new WaitUntilUTCDateTime(EndDateTime);
			EventTools.Fire(_onEnd);
			_offerRoutine = null;
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("StartDateTime", StartDateTime);
			storageDictionary.Set("EndDateTime", EndDateTime);
			return storageDictionary;
		}
	}
}
