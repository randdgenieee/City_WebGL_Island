using CIG;
using System.Collections.Generic;

public static class SpinnerManager
{
	private static readonly List<object> _spinRequesters = new List<object>();

	public static void PushSpinnerRequest(object requester)
	{
		if (_spinRequesters.Count == 0)
		{
		}
		if (!_spinRequesters.Contains(requester))
		{
			_spinRequesters.Add(requester);
		}
	}

	public static void PopSpinnerRequest(object requester)
	{
		if (_spinRequesters.Remove(requester) && _spinRequesters.Count == 0)
		{
		}
	}
}
