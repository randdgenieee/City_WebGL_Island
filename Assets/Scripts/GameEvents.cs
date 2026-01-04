using System;

public static class GameEvents
{
	private static EventDispatcher _dispatcher = new EventDispatcher();

	public static void Subscribe<T>(Action<T> callback) where T : class
	{
		_dispatcher.Subscribe(callback);
	}

	public static void Unsubscribe<T>(Action<T> callback) where T : class
	{
		_dispatcher.Unsubscribe(callback);
	}

	public static void Invoke<T>(T evt) where T : class
	{
		_dispatcher.Invoke(evt);
	}
}
