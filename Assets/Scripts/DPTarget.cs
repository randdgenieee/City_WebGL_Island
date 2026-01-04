using CIG;
using UnityEngine;

public class DPTarget : MonoBehaviour
{
	private const float ReferenceScreenDPI = 160f;

	private const float MinimumDP = 48f;

	private const float Multiplier = 0.3f;

	[SerializeField]
	private RectTransform _selfTransform;

	[SerializeField]
	private RectTransform _targetTransform;

	[SerializeField]
	private Vector2 _maxSize = new Vector2(80f, 80f);

	public void Initialize(CIGCanvasScaler canvasScaler)
	{
		float value = CIGGameConstants.CurrentDPI * 0.3f / canvasScaler.CanvasScaleFactor;
		_selfTransform.sizeDelta = new Vector2(Mathf.Clamp(value, _targetTransform.rect.width, _maxSize.x), Mathf.Clamp(value, _targetTransform.rect.height, _maxSize.y));
	}
}
