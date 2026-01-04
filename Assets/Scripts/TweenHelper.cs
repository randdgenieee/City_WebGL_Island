using UnityEngine;

public abstract class TweenHelper : MonoBehaviour
{
	public delegate void FinishedTweeningEventHandler();

	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private float _tweenTime = 1f;

	private decimal _startValue;

	private decimal _goalValue;

	private float _finishTime;

	public float CurrentTime
	{
		get;
		private set;
	}

	public bool Running => CurrentTime < _finishTime;

	public decimal CurrentValue
	{
		get
		{
			if (Mathf.Approximately(_finishTime, 0f))
			{
				return _goalValue;
			}
			float num = _curve.Evaluate(CurrentTime / _finishTime);
			if (float.IsNaN(num))
			{
				UnityEngine.Debug.LogErrorFormat("TweenHelper CurveResult is NaN after evaluating {0} / {1}", CurrentTime, _finishTime);
				return _goalValue;
			}
			return _startValue + (decimal)num * (_goalValue - _startValue);
		}
	}

	public event FinishedTweeningEventHandler FinishedTweeningEvent;

	private void FireFinishedTweeningEvent()
	{
		if (this.FinishedTweeningEvent != null)
		{
			this.FinishedTweeningEvent();
		}
	}

	private void Awake()
	{
		CurrentTime = _tweenTime;
		_finishTime = _tweenTime;
	}

	private void Update()
	{
		if (Running)
		{
			CurrentTime += Time.deltaTime;
			UpdateValue(CurrentValue);
			if (CurrentTime >= _finishTime)
			{
				FireFinishedTweeningEvent();
			}
		}
	}

	public void TweenTo(decimal goalValue)
	{
		TweenTo(goalValue, _tweenTime);
	}

	public void TweenTo(decimal goalValue, float time)
	{
		TweenTo(CurrentValue, goalValue, time);
	}

	public void TweenTo(decimal startValue, decimal goalValue)
	{
		TweenTo(startValue, goalValue, _tweenTime);
	}

	public void TweenTo(decimal startValue, decimal goalValue, float time)
	{
		_startValue = startValue;
		_goalValue = goalValue;
		_finishTime = time;
		if (Mathf.Approximately(time, 0f))
		{
			UpdateValue(_goalValue);
			FireFinishedTweeningEvent();
		}
		else
		{
			CurrentTime = 0f;
		}
	}

	public void StopTweening()
	{
		_finishTime = float.MinValue;
	}

	protected abstract void UpdateValue(decimal value);
}
