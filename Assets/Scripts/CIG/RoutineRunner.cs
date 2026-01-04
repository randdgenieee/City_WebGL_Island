using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public sealed class RoutineRunner
	{
		private readonly RoutineRunnerMonoBehaviour _runner;

		private readonly Dictionary<IEnumerator, IEnumerator> _routines = new Dictionary<IEnumerator, IEnumerator>();

		private readonly List<Action> _invokes = new List<Action>();

		public RoutineRunner(string name)
		{
			GameObject gameObject = new GameObject(name);
			_runner = gameObject.AddComponent<RoutineRunnerMonoBehaviour>();
		}

		public void Release()
		{
			if (_runner != null)
			{
				_runner.Release();
			}
			_invokes.Clear();
			_routines.Clear();
		}

		public void StartCoroutine(IEnumerator routine)
		{
			if (_runner == null || _runner.Destroyed)
			{
				UnityEngine.Debug.LogError("RoutineRunner is already destroyed. Cannot start new coroutine.");
				return;
			}
			if (_routines.ContainsKey(routine))
			{
				UnityEngine.Debug.LogWarning($"Routine '{routine}' already started. Cannot start again.");
				return;
			}
			IEnumerator enumerator = InternalRoutine(routine);
			_routines.Add(routine, enumerator);
			_runner.RunCoroutine(enumerator);
		}

		public void Invoke(Action action, float time, bool realtime = false)
		{
			if (_runner == null || _runner.Destroyed)
			{
				UnityEngine.Debug.LogError("RoutineRunner is already destroyed. Cannot invoke.");
				return;
			}
			_invokes.Add(action);
			_runner.RunInvoke(delegate
			{
				InvokeFinished(action);
			}, time, realtime);
		}

		public void InvokeRepeating(Action action, float time, float repeatRate, bool realtime = false)
		{
			if (_runner == null || _runner.Destroyed)
			{
				UnityEngine.Debug.LogError("RoutineRunner is already destroyed. Cannot invoke-repeating.");
				return;
			}
			_invokes.Add(action);
			_runner.RunInvokeRepeating(action, time, repeatRate, realtime);
		}

		public void CancelInvoke(Action action)
		{
			if (!(_runner == null) && !_runner.Destroyed)
			{
				_invokes.Remove(action);
				_runner.CancelInvoke(action);
			}
		}

		public void StopCoroutine(IEnumerator routine)
		{
			if (!(_runner == null) && !_runner.Destroyed)
			{
				if (!_routines.ContainsKey(routine))
				{
					UnityEngine.Debug.LogWarning($"Routine '{routine}' didn't start. Cannot stop it.");
					return;
				}
				_runner.StopCoroutine(_routines[routine]);
				_routines.Remove(routine);
			}
		}

		private IEnumerator InternalRoutine(IEnumerator routine)
		{
			while (_runner != null && routine.MoveNext())
			{
				object current = routine.Current;
				CustomYieldInstruction customYieldInstruction;
				if ((customYieldInstruction = (routine.Current as CustomYieldInstruction)) == null || customYieldInstruction.keepWaiting)
				{
					yield return current;
				}
			}
			RoutineFinished(routine);
		}

		private void RoutineFinished(IEnumerator routine)
		{
			if (_runner == null)
			{
				UnityEngine.Debug.LogWarning($"Routine '{routine}' was still running when RoutineRunner got destroyed.");
			}
			if (!_routines.ContainsKey(routine))
			{
				UnityEngine.Debug.LogWarning($"Routine '{routine}' finished but was not/no longer scheduled.");
			}
			else
			{
				_routines.Remove(routine);
			}
		}

		private void InvokeFinished(Action action)
		{
			_invokes.Remove(action);
			action();
		}
	}
}
