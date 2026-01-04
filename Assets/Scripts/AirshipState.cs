using CIG;
using System;
using System.Collections;

public class AirshipState : IStorable
{
	public delegate void StateChangedHandler(State previousState, State newState);

	public delegate void UpspeededEventHandler();

	public enum State
	{
		Travelling,
		Hovering,
		Landed
	}

	private readonly StorageDictionary _storage;

	private readonly Timing _timing;

	private readonly RoutineRunner _routineRunner;

	private readonly Multipliers _multipliers;

	private readonly GameState _gameState;

	private readonly IslandsManager _islandsManager;

	private State _state = State.Hovering;

	private const string StateKey = "State";

	private const string TravelDurationKey = "TravelDuration";

	private const string TravelUpspeedableProcessKey = "TravelUpspeedableProcess";

	private const string FromIslandKey = "FromIsland";

	private const string ToIslandKey = "ToIsland";

	private const string CurrentIslandKey = "CurrentIsland";

	public State CurrentState
	{
		get
		{
			return _state;
		}
		set
		{
			State state = _state;
			_state = value;
			if (_state != state)
			{
				FireStateChangedEvent(state, _state);
			}
		}
	}

	public double TravelTimeLeft
	{
		get
		{
			if (TravelUpspeedableProcess != null)
			{
				return TravelUpspeedableProcess.TimeLeft;
			}
			return TravelDuration;
		}
	}

	public IslandId CurrentIslandId
	{
		get;
		private set;
	}

	public IslandId FromIslandId
	{
		get;
		private set;
	}

	public IslandId ToIslandId
	{
		get;
		private set;
	}

	public double TravelDuration
	{
		get;
		private set;
	}

	public bool CanTravel => _state == State.Landed;

	public UpspeedableProcess TravelUpspeedableProcess
	{
		get;
		private set;
	}

	public event StateChangedHandler StateChangedEvent;

	public event UpspeededEventHandler UpspeededEvent;

	private void FireStateChangedEvent(State previousState, State newState)
	{
		this.StateChangedEvent?.Invoke(previousState, newState);
	}

	public void FireUpspeededEvent()
	{
		this.UpspeededEvent?.Invoke();
	}

	public AirshipState(StorageDictionary storage, Timing timing, RoutineRunner routineRunner, Multipliers multipliers, GameState gameState, IslandsManager islandsManager)
	{
		_storage = storage;
		_timing = timing;
		_routineRunner = routineRunner;
		_multipliers = multipliers;
		_gameState = gameState;
		_islandsManager = islandsManager;
		CurrentState = (State)_storage.Get("State", 1);
		TravelDuration = _storage.Get("TravelDuration", 0.0);
		FromIslandId = (IslandId)_storage.Get("FromIsland", -1);
		ToIslandId = (IslandId)_storage.Get("ToIsland", -1);
		CurrentIslandId = (IslandId)_storage.Get("CurrentIsland", 0);
		if (_storage.Contains("TravelUpspeedableProcess"))
		{
			TravelUpspeedableProcess = new UpspeedableProcess(_storage.GetStorageDict("TravelUpspeedableProcess"), _timing, _multipliers, _gameState);
			TravelUpspeedableProcess.UpspeededEvent += OnUpspeeded;
		}
		if (CurrentState == State.Travelling && TravelUpspeedableProcess != null)
		{
			_routineRunner.StartCoroutine(Travel());
		}
		else if (CurrentState == State.Hovering)
		{
			_islandsManager.IslandChangedEvent += OnIslandChangedWhileHovering;
		}
	}

	public void StartTravelling(IslandId to, int duration, Currency cost, CurrenciesSpentReason spentReason, Action<bool> callback)
	{
		if (CurrentState == State.Landed)
		{
			_gameState.SpendCurrencies(cost, spentReason, delegate(bool success, Currencies spent)
			{
				if (success)
				{
					TravelDuration = duration;
					FromIslandId = CurrentIslandId;
					ToIslandId = to;
					CurrentIslandId = IslandId.None;
					AnalyticsLogTravel();
					TravelUpspeedableProcess = new UpspeedableProcess(_timing, _multipliers, _gameState, TravelDuration, CurrenciesSpentReason.SpeedupAirship);
					TravelUpspeedableProcess.UpspeededEvent += OnUpspeeded;
					CurrentState = State.Travelling;
					_routineRunner.StartCoroutine(Travel());
				}
				callback?.Invoke(success);
			});
		}
	}

	public void InstantTravel(IslandId to, Currency cost, CurrenciesSpentReason spentReason, Action<bool> callback)
	{
		_gameState.SpendCurrencies(cost, spentReason, delegate(bool success, Currencies spent)
		{
			if (success)
			{
				FromIslandId = CurrentIslandId;
				ToIslandId = to;
				TravelDuration = 8.0;
				CurrentIslandId = IslandId.None;
				CurrentState = State.Travelling;
				FireUpspeededEvent();
				AnalyticsLogTravel();
				FinishTravel();
			}
			callback?.Invoke(success);
		});
	}

	public void OverrideTravelDuration(double duration)
	{
		TravelUpspeedableProcess?.OverrideDuration(duration);
	}

	public Currency GetGoldCostForTravelDuration(long duration)
	{
		return Currency.GoldCurrency(GoldCostUtility.GetSpeedupCostGoldForSeconds(_multipliers, duration));
	}

	private void OnUpspeeded()
	{
		FireUpspeededEvent();
	}

	private IEnumerator Travel()
	{
		yield return TravelUpspeedableProcess;
		FinishTravel();
	}

	private void FinishTravel()
	{
		CurrentIslandId = ToIslandId;
		FromIslandId = IslandId.None;
		ToIslandId = IslandId.None;
		TravelDuration = double.MinValue;
		if (TravelUpspeedableProcess != null)
		{
			TravelUpspeedableProcess.UpspeededEvent -= OnUpspeeded;
			TravelUpspeedableProcess = null;
		}
		if (!_islandsManager.IsUnlocked(CurrentIslandId))
		{
			_islandsManager.UnlockIsland(CurrentIslandId);
			_islandsManager.IslandChangedEvent += OnIslandChangedWhileHovering;
			CurrentState = State.Hovering;
		}
		else
		{
			CurrentState = State.Landed;
		}
	}

	private void OnIslandChangedWhileHovering(IslandId islandId, bool isVisiting)
	{
		if (!isVisiting && islandId == CurrentIslandId)
		{
			CurrentState = State.Landed;
			_islandsManager.IslandChangedEvent -= OnIslandChangedWhileHovering;
		}
	}

	private void AnalyticsLogTravel()
	{
		Analytics.AirshipSent(FromIslandId.ToString(), ToIslandId.ToString());
	}

	public StorageDictionary Serialize()
	{
		_storage.Set("State", (int)CurrentState);
		_storage.Set("TravelDuration", TravelDuration);
		_storage.Set("FromIsland", (int)FromIslandId);
		_storage.Set("ToIsland", (int)ToIslandId);
		_storage.Set("CurrentIsland", (int)CurrentIslandId);
		_storage.SetOrRemoveStorable("TravelUpspeedableProcess", TravelUpspeedableProcess, TravelUpspeedableProcess == null);
		return _storage;
	}
}
