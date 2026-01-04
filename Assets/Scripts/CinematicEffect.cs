using CIG;
using System.Collections;
using UnityEngine;

public class CinematicEffect : SingletonMonobehaviour<CinematicEffect>
{
	public const float FadeDurationInSeconds = 1.2f;

	private const float MinScale = 3f;

	private const float LoadScale = 5f;

	private const float MaxScale = 7f;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Transform _background;

	[SerializeField]
	private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private IEnumerator _fadeRoutine;

	public bool IsShowing => _camera.enabled;

	protected override void Awake()
	{
		base.Awake();
		HideInstant();
	}

	public Coroutine ShowAnimated(bool scaleUp)
	{
		StopCurrentRoutine();
		return StartCoroutine(_fadeRoutine = ShowRoutine(scaleUp ? 3f : 7f));
	}

	public Coroutine HideAnimated(bool scaleUp)
	{
		StopCurrentRoutine();
		return StartCoroutine(_fadeRoutine = HideRoutine(scaleUp ? 7f : 3f));
	}

	public void ShowInstant()
	{
		StopCurrentRoutine();
		_camera.enabled = true;
		_canvasGroup.blocksRaycasts = true;
		_canvasGroup.alpha = 1f;
	}

	public void HideInstant()
	{
		StopCurrentRoutine();
		_camera.enabled = false;
		_canvasGroup.blocksRaycasts = false;
		_canvasGroup.alpha = 0f;
	}

	private IEnumerator ShowRoutine(float fromScale)
	{
		_camera.enabled = true;
		_canvasGroup.blocksRaycasts = true;
		float time = 0f;
		while (time < 1.2f)
		{
			time += Time.unscaledDeltaTime;
			float num = time / 1.2f;
			_canvasGroup.alpha = _fadeCurve.Evaluate(num);
			_background.localScale = Mathf.Lerp(fromScale, 5f, num) * Vector3.one;
			yield return null;
		}
		_canvasGroup.alpha = 1f;
		SpinnerManager.PushSpinnerRequest(this);
	}

	private IEnumerator HideRoutine(float fromScale)
	{
		SpinnerManager.PopSpinnerRequest(this);
		float time = 1.2f;
		while (time > 0f)
		{
			time -= Time.unscaledDeltaTime;
			float num = time / 1.2f;
			_canvasGroup.alpha = _fadeCurve.Evaluate(num);
			_background.localScale = Mathf.Lerp(fromScale, 5f, num) * Vector3.one;
			yield return null;
		}
		_canvasGroup.alpha = 0f;
		_camera.enabled = false;
		_canvasGroup.blocksRaycasts = false;
	}

	private void StopCurrentRoutine()
	{
		if (_fadeRoutine != null)
		{
			StopCoroutine(_fadeRoutine);
		}
		SpinnerManager.PopSpinnerRequest(this);
	}
}
