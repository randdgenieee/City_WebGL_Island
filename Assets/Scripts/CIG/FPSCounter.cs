using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class FPSCounter : MonoBehaviour
	{
		private const float UpdateFPSInterval = 0.1f;

		private static readonly Color Orange = new Color(1f, 0.647058845f, 0f);

		private static FPSCounter _instance = null;

		private float _timer;

		private float _deltaTime;

		private int _frameCount;

		private int _fps;

		private Text _label;

		public static void Create()
		{
		}

		private void Start()
		{
			Canvas canvas = base.gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 32767;
			CanvasScaler canvasScaler = base.gameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			canvasScaler.matchWidthOrHeight = 0.5f;
			canvasScaler.referencePixelsPerUnit = 100f;
			GameObject gameObject = new GameObject("Label");
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = Vector3.one;
			_label = gameObject.AddComponent<Text>();
			Outline outline = gameObject.AddComponent<Outline>();
			string[] oSInstalledFontNames = Font.GetOSInstalledFontNames();
			_label.font = Font.CreateDynamicFontFromOSFont(oSInstalledFontNames[0], 30);
			_label.fontStyle = FontStyle.Bold;
			_label.fontSize = 30;
			_label.color = Color.green;
			outline.effectColor = Color.black;
			outline.effectDistance = new Vector2(1f, -1f);
			RectTransform rectTransform = _label.rectTransform;
			RectTransform rectTransform2 = _label.rectTransform;
			Vector2 vector = _label.rectTransform.pivot = Vector2.up;
			Vector2 vector4 = rectTransform.anchorMin = (rectTransform2.anchorMax = vector);
			_label.rectTransform.anchoredPosition = Vector3.one * 5f;
		}

		private void Update()
		{
			_deltaTime += Time.unscaledDeltaTime;
			_frameCount++;
			_timer += Time.unscaledDeltaTime;
			if (_timer > 0.1f)
			{
				_timer %= 0.1f;
				CalculateFramerate();
			}
		}

		private void CalculateFramerate()
		{
			if (_frameCount != 0 && !Mathf.Approximately(_deltaTime, 0f))
			{
				_fps = Mathf.FloorToInt((float)_frameCount / _deltaTime);
				_label.text = _fps.ToString();
				Color colorForFPS = GetColorForFPS(_fps);
				if (_label.color != colorForFPS)
				{
					_label.color = colorForFPS;
				}
				_frameCount = 0;
				_deltaTime = 0f;
			}
		}

		private Color GetColorForFPS(int fps)
		{
			if (fps < 15)
			{
				return Color.red;
			}
			if (fps < 25)
			{
				return Orange;
			}
			if (fps < 45)
			{
				return Color.yellow;
			}
			return Color.green;
		}
	}
}
