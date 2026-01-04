using System.Collections;
using UnityEngine;

namespace CIG
{
	public class WeatherManager : MonoBehaviour
	{
		[SerializeField]
		private WeatherState[] _states;

		private Timing _timing;

		private RoutineRunner _routineRunner;

		private readonly WeightedList<int> _weightedStateIndices = new WeightedList<int>();

		private int _currentStateIndex;

		private double _stateStartTime;

		private double _stateDuration;

		private IEnumerator _weatherRoutine;

		private const string CurrentStateKey = "CurrentState";

		private const string StateStartTimeKey = "StateStartTime";

		private const string StateDurationKey = "StateDuration";

		private StorageDictionary _storage;

		public bool CanSwitchStates => _states.Length > 1;

		public void Initialize(StorageDictionary storage, Timing timing, RoutineRunner routineRunner)
		{
			_timing = timing;
			_routineRunner = routineRunner;
			int i = 0;
			for (int num = _states.Length; i < num; i++)
			{
				_weightedStateIndices.Add(i, _states[i].Weight);
			}
			Deserialize(storage);
			int j = 0;
			for (int num2 = _states.Length; j < num2; j++)
			{
				_states[j].Initialize(_timing, _routineRunner);
			}
			if (CanSwitchStates)
			{
				DetermineState();
				_routineRunner.StartCoroutine(_weatherRoutine = WeatherRoutine());
			}
			else if (_states.Length != 0)
			{
				_stateStartTime = 0.0;
				_stateDuration = 0.0;
				_states[0].EnterStateNow();
			}
		}

		private void OnDestroy()
		{
			if (_routineRunner != null && _weatherRoutine != null)
			{
				_routineRunner.StopCoroutine(_weatherRoutine);
				_weatherRoutine = null;
			}
		}

		private void DetermineState()
		{
			double gameTime = _timing.GameTime;
			if (gameTime < _stateStartTime || gameTime > _stateStartTime + _stateDuration)
			{
				_currentStateIndex = _weightedStateIndices.PickRandom();
				_stateStartTime = gameTime;
				_stateDuration = _states[_currentStateIndex].GetDuration();
				Serialize();
			}
		}

		private IEnumerator WeatherRoutine()
		{
			while (true)
			{
				yield return _states[_currentStateIndex].StateRoutine(_stateStartTime, _stateDuration);
				DetermineState();
			}
		}

		private void Serialize()
		{
			_storage.Set("CurrentState", _currentStateIndex);
			_storage.Set("StateStartTime", _stateStartTime);
			_storage.Set("StateDuration", _stateDuration);
		}

		private void Deserialize(StorageDictionary storage)
		{
			_storage = storage;
			_currentStateIndex = _storage.Get("CurrentState", 0);
			_stateStartTime = _storage.Get("StateStartTime", 0.0);
			_stateDuration = _storage.Get("StateDuration", 0.0);
		}
	}
}
