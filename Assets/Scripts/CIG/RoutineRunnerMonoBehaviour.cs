using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CIG
{
	public class RoutineRunnerMonoBehaviour : MonoBehaviour
	{
		private class PendingInvoke
		{
			public Action Action
			{
				get;
			}

			public float Time
			{
				get;
			}

			public float? RepeatRate
			{
				get;
			}

			public bool RealTime
			{
				get;
			}

			public PendingInvoke(Action action, float time, bool realTime, float? repeatRate)
			{
				Action = action;
				Time = time;
				RealTime = realTime;
				RepeatRate = repeatRate;
			}
		}

		private readonly Queue<IEnumerator> _pendingRoutines = new Queue<IEnumerator>();

		private readonly Queue<PendingInvoke> _pendingInvokes = new Queue<PendingInvoke>();

		private int _mainThreadId;

		public bool Destroyed
		{
			get;
			private set;
		}

		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void Release()
		{
			StopAllCoroutines();
			UnityEngine.Object.Destroy(base.gameObject);
			Destroyed = true;
		}

		private void Update()
		{
			lock (_pendingRoutines)
			{
				while (_pendingRoutines.Count > 0)
				{
					StartCoroutine(_pendingRoutines.Dequeue());
				}
			}
			lock (_pendingInvokes)
			{
				while (_pendingInvokes.Count > 0)
				{
					PendingInvoke pendingInvoke = _pendingInvokes.Dequeue();
					if (pendingInvoke.RepeatRate.HasValue)
					{
						this.InvokeRepeating(pendingInvoke.Action, pendingInvoke.Time, pendingInvoke.RepeatRate.Value, pendingInvoke.RealTime);
					}
					else
					{
						this.Invoke(pendingInvoke.Action, pendingInvoke.Time, pendingInvoke.RealTime);
					}
				}
			}
		}

		public void RunCoroutine(IEnumerator routine)
		{
			if (IsOnMainThread())
			{
				StartCoroutine(routine);
			}
			else
			{
				lock (_pendingRoutines)
				{
					_pendingRoutines.Enqueue(routine);
				}
			}
		}

		public void RunInvoke(Action action, float time, bool realTime)
		{
			if (IsOnMainThread())
			{
				this.Invoke(action, time, realTime);
			}
			else
			{
				lock (_pendingRoutines)
				{
					_pendingInvokes.Enqueue(new PendingInvoke(action, time, realTime, null));
				}
			}
		}

		public void RunInvokeRepeating(Action action, float time, float repeatRate, bool realTime)
		{
			if (IsOnMainThread())
			{
				this.InvokeRepeating(action, time, repeatRate, realTime);
			}
			else
			{
				lock (_pendingInvokes)
				{
					_pendingInvokes.Enqueue(new PendingInvoke(action, time, realTime, repeatRate));
				}
			}
		}

		private bool IsOnMainThread()
		{
			return _mainThreadId == Thread.CurrentThread.ManagedThreadId;
		}
	}
}
