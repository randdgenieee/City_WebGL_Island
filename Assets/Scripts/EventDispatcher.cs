using System;
using System.Collections.Generic;

public class EventDispatcher
{
	private Dictionary<Type, HashSet<Action<object>>> _events = new Dictionary<Type, HashSet<Action<object>>>();

	private Dictionary<object, Action<object>> _lookupTable = new Dictionary<object, Action<object>>();

	public void Subscribe<T>(Action<T> callback) where T : class
	{
		Action<object> action = delegate(object o)
		{
			callback((T)o);
		};
		if (!_events.ContainsKey(typeof(T)))
		{
			_events[typeof(T)] = new HashSet<Action<object>>();
		}
		_events[typeof(T)].Add(action);
		_lookupTable.Add(callback, action);
	}

	public void Unsubscribe<T>(Action<T> callback) where T : class
	{
		if (_events.ContainsKey(typeof(T)))
		{
			_events[typeof(T)].Remove(_lookupTable[callback]);
		}
		_lookupTable.Remove(callback);
	}

	public void Invoke<T>(T evt) where T : class
	{
		if (_events.TryGetValue(typeof(T), out HashSet<Action<object>> value) && value != null)
		{
			foreach (Action<object> item in value)
			{
				item(evt);
			}
		}
	}
}
