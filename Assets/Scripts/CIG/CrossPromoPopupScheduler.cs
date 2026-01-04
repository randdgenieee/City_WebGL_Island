using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
	public class CrossPromoPopupScheduler
	{
		public static readonly TimeSpan DaysBetweenPromoPopup = TimeSpan.FromDays(2.0);

		private readonly StorageDictionary _storage;

		private readonly PopupManager _popupManager;

		private readonly WorldMap _worldMap;

		private readonly GameState _gameState;

		private readonly CrossPromo _crossPromo;

		private DateTime _lastPromoPopupDay;

		private bool _isWaitingToShowPopup;

		private const string LastPromoPopupKey = "LastPromoPopupKey";

		private float TimeToNextPopup => (float)(_lastPromoPopupDay + DaysBetweenPromoPopup - AntiCheatDateTime.UtcNow).TotalSeconds;

		private bool CanShowPopup
		{
			get
			{
				if (Time.timeSinceLevelLoad > 60f && !_worldMap.IsVisible && _gameState.Level >= 10)
				{
					return TimeToNextPopup <= 0f;
				}
				return false;
			}
		}

		public CrossPromoPopupScheduler(StorageDictionary storage, RoutineRunner routineRunner, PopupManager popupManager, WorldMap worldMap, GameState gameState, CrossPromo crossPromo)
		{
			_storage = storage;
			_popupManager = popupManager;
			_worldMap = worldMap;
			_gameState = gameState;
			_crossPromo = crossPromo;
			_lastPromoPopupDay = _storage.GetDateTime("LastPromoPopupKey", AntiCheatDateTime.UtcNow - DaysBetweenPromoPopup);
			routineRunner.StartCoroutine(ShowCrossPromoRoutine());
		}

		public void ShowedPromo()
		{
			_isWaitingToShowPopup = false;
			_lastPromoPopupDay = AntiCheatDateTime.UtcNow;
		}

		private void ShowCrossPromo(SparkSocGame game)
		{
			_popupManager.RequestPopup(new PromoPopupRequest(game));
		}

		private IEnumerator ShowCrossPromoRoutine()
		{
			while (true)
			{
				bool isGettingPromoItem = true;
				SparkSocGame game = null;
				_crossPromo.GetPromoItem(delegate(SparkSocGame g)
				{
					game = g;
					isGettingPromoItem = false;
				});
				while (isGettingPromoItem || !CanShowPopup)
				{
					yield return new WaitForSecondsRealtime(Mathf.Max(5f, TimeToNextPopup));
				}
				if (game != null)
				{
					_isWaitingToShowPopup = true;
					ShowCrossPromo(game);
					yield return new WaitWhile(() => _isWaitingToShowPopup);
				}
				else
				{
					yield return new WaitForSecondsRealtime(60f);
				}
			}
		}

		public void Serialize()
		{
			_storage.Set("LastPromoPopupKey", _lastPromoPopupDay);
		}
	}
}
