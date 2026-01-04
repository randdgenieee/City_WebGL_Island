using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public abstract class WalkerBalloon
	{
		public delegate void ExpiredEventHandler(WalkerBalloon walkerBalloon);

		public delegate void CurrenciesCollectedEventHandler(Currency currency, Clip sound);

		private const string MinutesPlayedAtLastClickKey = "MinutesPlayedAtLastClick";

		private const string MinutesPlayedAtLastShownKey = "MinutesPlayedAtLastShown";

		private readonly StorageDictionary _storage;

		private readonly RoutineRunner _routineRunner;

		protected readonly GameState _gameState;

		protected readonly PopupManager _popupManager;

		protected readonly WalkerBalloonProperties _properties;

		private IEnumerator _expireRoutine;

		private bool _expired;

		public WalkerBalloonType WalkerBalloonType => _properties.BalloonType;

		public DateTime ExpireTime
		{
			get;
		}

		public virtual bool IsAvailable
		{
			get
			{
				if (_gameState.TotalMinutesPlayed - MinutesPlayedAtLastClick < _properties.CooldownAfterClickMinutes)
				{
					return false;
				}
				if (_gameState.TotalMinutesPlayed - MinutesPlayedAtLastShown < _properties.CooldownAfterShowMinutes)
				{
					return false;
				}
				return true;
			}
		}

		private long MinutesPlayedAtLastClick
		{
			get
			{
				return _storage.Get("MinutesPlayedAtLastClick", 0L);
			}
			set
			{
				_storage.Set("MinutesPlayedAtLastClick", value);
			}
		}

		private long MinutesPlayedAtLastShown
		{
			get
			{
				return _storage.Get("MinutesPlayedAtLastShown", 0L);
			}
			set
			{
				_storage.Set("MinutesPlayedAtLastShown", value);
			}
		}

		public event ExpiredEventHandler ExpiredEvent;

		public event CurrenciesCollectedEventHandler CurrenciesCollectedEvent;

		private void FireExpiredEvent()
		{
			this.ExpiredEvent?.Invoke(this);
		}

		protected void FireCurrenciesCollectedEvent(Currency currency, Clip sound)
		{
			this.CurrenciesCollectedEvent?.Invoke(currency, sound);
		}

		protected WalkerBalloon(WalkerBalloonProperties properties, GameState gameState, PopupManager popupManager, RoutineRunner routineRunner)
		{
			_storage = StorageController.GameRoot.GetStorageDict(properties.GroupType.ToString());
			_gameState = gameState;
			_popupManager = popupManager;
			_routineRunner = routineRunner;
			_properties = properties;
			ExpireTime = AntiCheatDateTime.UtcNow.AddSeconds(properties.ShowDurationSeconds);
		}

		public void Shown()
		{
			if (_expireRoutine == null)
			{
				MinutesPlayedAtLastShown = _gameState.TotalMinutesPlayed;
				_routineRunner.StartCoroutine(_expireRoutine = ExpireRoutine(_properties.ShowDurationSeconds));
			}
		}

		public void Collect()
		{
			if (!_expired)
			{
				OnCollected();
				Expire();
			}
		}

		protected virtual void OnCollected()
		{
			MinutesPlayedAtLastClick = _gameState.TotalMinutesPlayed;
		}

		protected virtual void OnExpired()
		{
			_expired = true;
		}

		protected void Expire()
		{
			if (_expireRoutine != null)
			{
				_routineRunner.StopCoroutine(_expireRoutine);
				_expireRoutine = null;
			}
			OnExpired();
			FireExpiredEvent();
		}

		protected void ShowPopup(string analyticsScreenName, string bodyKey, Sprite icon)
		{
			GenericPopupRequest request = new GenericPopupRequest(analyticsScreenName).SetTexts(Localization.Key("city_advisor"), Localization.Key(bodyKey)).SetGreenOkButton(PopupAction).SetRedButton(Localization.Key("perhaps_later"))
				.SetIcon(icon);
			_popupManager.RequestPopup(request);
		}

		protected virtual void PopupAction()
		{
		}

		private IEnumerator ExpireRoutine(double duration)
		{
			DateTime endDateTime = AntiCheatDateTime.UtcNow.AddSeconds(duration);
			yield return new WaitUntilUTCDateTime(endDateTime);
			_expireRoutine = null;
			Expire();
		}
	}
}
