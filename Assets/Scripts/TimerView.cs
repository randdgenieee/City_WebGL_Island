using System.Collections;
using UnityEngine;

public abstract class TimerView : MonoBehaviour
{
	protected bool _isActive;

	private IEnumerator _timerRoutine;

	protected abstract float TimeLeftSeconds
	{
		get;
	}

	private bool HasTimeRemaining => TimeLeftSeconds > 0f;

	private void OnEnable()
	{
		UpdateState();
	}

	private void OnDisable()
	{
		if (_timerRoutine != null)
		{
			StopCoroutine(_timerRoutine);
			_timerRoutine = null;
		}
	}

	public virtual void StopTimer()
	{
		_isActive = false;
		UpdateState();
	}

	protected void StartTimer()
	{
		_isActive = true;
		UpdateState();
	}

	protected virtual void UpdateState()
	{
		if (HasTimeRemaining && base.gameObject.activeInHierarchy)
		{
			if (_timerRoutine != null)
			{
				StopCoroutine(_timerRoutine);
			}
			StartCoroutine(_timerRoutine = TimerRoutine());
		}
		else if (_timerRoutine != null)
		{
			StopCoroutine(_timerRoutine);
			_timerRoutine = null;
		}
	}

	protected abstract void UpdateTime(float timeSpan);

	private IEnumerator TimerRoutine()
	{
		while (HasTimeRemaining)
		{
			UpdateTime(TimeLeftSeconds);
			yield return new WaitForSeconds(1f);
		}
		UpdateTime(0f);
		_timerRoutine = null;
	}
}
