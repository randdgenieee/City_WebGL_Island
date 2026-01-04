using UnityEngine;

namespace CIG
{
	public class SafeAreaRect : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _rectTransform;

		[SerializeField]
		private bool _activateOnStart = true;

		public bool Enabled
		{
			get;
			private set;
		}

		private void Start()
		{
			if (_activateOnStart)
			{
				Enable();
			}
		}

		public void Enable()
		{
			if (!Enabled)
			{
				SetSafeAreaToUnitySafeArea();
				Enabled = true;
			}
		}

		public void Disable()
		{
			if (Enabled)
			{
				SetSafeArea(Vector2.zero, Vector2.one);
				Enabled = false;
			}
		}

		private void SetSafeAreaToUnitySafeArea()
		{
			Vector2 position = new Vector2(Screen.safeArea.position.x / (float)Screen.width, Screen.safeArea.position.y / (float)Screen.height);
			Vector2 size = new Vector2(Screen.safeArea.size.x / (float)Screen.width, Screen.safeArea.size.y / (float)Screen.height);
			SetSafeArea(position, size);
		}

		private void SetSafeArea(Vector2 position, Vector2 size)
		{
			_rectTransform.anchorMin = position;
			_rectTransform.anchorMax = position + size;
		}
	}
}
