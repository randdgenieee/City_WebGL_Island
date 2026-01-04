using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public static class ActionInvoker
	{
		private static readonly Dictionary<Action, IEnumerator> _invokeMethods = new Dictionary<Action, IEnumerator>();

		public static void Invoke(this MonoBehaviour monoBehaviour, Action action, float time, bool realtime = false)
		{
			IEnumerator enumerator = InvokeCoroutine(action, time, realtime);
			if (_invokeMethods.ContainsKey(action))
			{
				UnityEngine.Debug.LogWarning($"[ActionInvoker] Overriding enumerator for action '{action.Method}'.");
				_invokeMethods[action] = enumerator;
			}
			else
			{
				_invokeMethods.Add(action, enumerator);
			}
			monoBehaviour.StartCoroutine(enumerator);
		}

		public static void InvokeNextFrame(this MonoBehaviour monoBehaviour, Action action)
		{
			IEnumerator enumerator = InvokeCoroutine(action);
			if (_invokeMethods.ContainsKey(action))
			{
				UnityEngine.Debug.LogWarning($"[ActionInvoker] Overriding enumerator for action '{action.Method}'.");
				_invokeMethods[action] = enumerator;
			}
			else
			{
				_invokeMethods.Add(action, enumerator);
			}
			monoBehaviour.StartCoroutine(enumerator);
		}

		public static void InvokeRepeating(this MonoBehaviour monoBehaviour, Action action, float time, float repeatRate, bool realtime = false)
		{
			IEnumerator enumerator = InvokeRepeatingCoroutine(action, time, repeatRate, realtime);
			if (_invokeMethods.ContainsKey(action))
			{
				UnityEngine.Debug.LogWarning($"[ActionInvoker] Overriding enumerator for action '{action.Method}'.");
				_invokeMethods[action] = enumerator;
			}
			else
			{
				_invokeMethods.Add(action, enumerator);
			}
			monoBehaviour.StartCoroutine(enumerator);
		}

		public static bool IsInvoking(this MonoBehaviour monoBehaviour, Action action)
		{
			return _invokeMethods.ContainsKey(action);
		}

		public static void CancelInvoke(this MonoBehaviour monoBehaviour, Action action)
		{
			if (_invokeMethods.ContainsKey(action))
			{
				monoBehaviour.StopCoroutine(_invokeMethods[action]);
				_invokeMethods.Remove(action);
			}
		}

		private static IEnumerator InvokeCoroutine(Action action, float time, bool realtime)
		{
			if (realtime)
			{
				yield return new WaitForSecondsRealtime(time);
			}
			else
			{
				yield return new WaitForSeconds(time);
			}
			if (_invokeMethods.ContainsKey(action))
			{
				_invokeMethods.Remove(action);
				action();
			}
		}

		private static IEnumerator InvokeCoroutine(Action action)
		{
			yield return null;
			if (_invokeMethods.ContainsKey(action))
			{
				_invokeMethods.Remove(action);
				action();
			}
		}

		private static IEnumerator InvokeRepeatingCoroutine(Action action, float time, float repeatRate, bool realtime)
		{
			if (realtime)
			{
				yield return new WaitForSecondsRealtime(time);
			}
			else
			{
				yield return new WaitForSeconds(time);
			}
			if (!_invokeMethods.ContainsKey(action))
			{
				yield break;
			}
			while (_invokeMethods.ContainsKey(action))
			{
				action();
				if (realtime)
				{
					yield return new WaitForSecondsRealtime(repeatRate);
				}
				else
				{
					yield return new WaitForSeconds(repeatRate);
				}
			}
		}
	}
}
