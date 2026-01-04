using UnityEngine;
using UnityEngine.UI;

namespace Tweening.Extensions
{
	public static class Extensions
	{
		public static void UpdateLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.x = x;
			transform.localPosition = localPosition;
		}

		public static void UpdatePositionX(this Transform transform, float x)
		{
			Vector3 position = transform.position;
			position.x = x;
			transform.position = position;
		}

		public static void UpdatePositionY(this Transform transform, float y)
		{
			Vector3 position = transform.position;
			position.y = y;
			transform.position = position;
		}

		public static void UpdatePositionZ(this Transform transform, float z)
		{
			Vector3 position = transform.position;
			position.z = z;
			transform.position = position;
		}

		public static void UpdateLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = transform.localScale;
			localScale.x = x;
			transform.localScale = localScale;
		}

		public static void UpdateLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = transform.localScale;
			localScale.y = y;
			transform.localScale = localScale;
		}

		public static void UpdateLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = transform.localScale;
			localScale.z = z;
			transform.localScale = localScale;
		}

		public static void UpdateLocalEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.x = x;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void UpdateLocalEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.y = y;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void UpdateLocalEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.z = z;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void UpdateAnchoredPositionX(this RectTransform transform, float x)
		{
			Vector3 anchoredPosition3D = transform.anchoredPosition3D;
			anchoredPosition3D.x = x;
			transform.anchoredPosition3D = anchoredPosition3D;
		}

		public static void UpdateAnchoredPositionY(this RectTransform transform, float y)
		{
			Vector3 anchoredPosition3D = transform.anchoredPosition3D;
			anchoredPosition3D.y = y;
			transform.anchoredPosition3D = anchoredPosition3D;
		}

		public static void UpdateAnchoredPositionZ(this RectTransform transform, float z)
		{
			Vector3 anchoredPosition3D = transform.anchoredPosition3D;
			anchoredPosition3D.z = z;
			transform.anchoredPosition3D = anchoredPosition3D;
		}

		public static void UpdateSizeDeltaX(this RectTransform transform, float x)
		{
			Vector2 sizeDelta = transform.sizeDelta;
			sizeDelta.x = x;
			transform.sizeDelta = sizeDelta;
		}

		public static void UpdateSizeDeltaY(this RectTransform transform, float y)
		{
			Vector2 sizeDelta = transform.sizeDelta;
			sizeDelta.y = y;
			transform.sizeDelta = sizeDelta;
		}

		public static void UpdateColorAlpha(this Image image, float a)
		{
			Color color = image.color;
			color.a = a;
			image.color = color;
		}

		public static void UpdateColorAlpha(this Outline outline, float a)
		{
			Color effectColor = outline.effectColor;
			effectColor.a = a;
			outline.effectColor = effectColor;
		}

		public static void UpdateColorAlpha(this Shadow shadow, float a)
		{
			Color effectColor = shadow.effectColor;
			effectColor.a = a;
			shadow.effectColor = effectColor;
		}

		public static void UpdateColorAlpha(this SpriteRenderer spriteRenderer, float a)
		{
			Color color = spriteRenderer.color;
			color.a = a;
			spriteRenderer.color = color;
		}

		public static void UpdateColorAlpha(this Text text, float a)
		{
			Color color = text.color;
			color.a = a;
			text.color = color;
		}
	}
}
