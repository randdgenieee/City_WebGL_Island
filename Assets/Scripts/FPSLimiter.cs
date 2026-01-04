using CIG;
using System.Collections.Generic;
using UnityEngine;

public sealed class FPSLimiter : SingletonMonobehaviour<FPSLimiter>
{
	private const int UnlimitedFPSDefault = 60;

	private const int LimitedFPSDefault = 30;

	private List<object> _unlimitedFPSRequesters = new List<object>();

	private int LimitedFPS
	{
		get;
		set;
	}

	private int UnlimitedFPS
	{
		get;
		set;
	}

	protected override void Awake()
	{
		base.Awake();
		Object.DontDestroyOnLoad(this);
		LimitedFPS = 30;
		UnlimitedFPS = 60;
		Application.targetFrameRate = LimitedFPS;
	}

	public void PushUnlimitedFPSRequest(object requester)
	{
		if (_unlimitedFPSRequesters.Count == 0)
		{
			Application.targetFrameRate = UnlimitedFPS;
		}
		if (!_unlimitedFPSRequesters.Contains(requester))
		{
			_unlimitedFPSRequesters.Add(requester);
		}
	}

	public void PopUnlimitedFPSRequest(object requester)
	{
		_unlimitedFPSRequesters.Remove(requester);
		if (_unlimitedFPSRequesters.Count == 0)
		{
			Application.targetFrameRate = LimitedFPS;
		}
	}
}
