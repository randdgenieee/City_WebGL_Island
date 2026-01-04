using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class MaskView : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _rectTransform;

		[SerializeField]
		private Image _top;

		[SerializeField]
		private Image _bottom;

		[SerializeField]
		private Image _left;

		[SerializeField]
		private Image _right;

		public void Show(Vector2 size, bool clickable)
		{
			_rectTransform.sizeDelta = size;
			base.gameObject.SetActive(value: true);
			SetClickable(clickable);
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}

		private void SetClickable(bool clickable)
		{
			_top.raycastTarget = clickable;
			_bottom.raycastTarget = clickable;
			_left.raycastTarget = clickable;
			_right.raycastTarget = clickable;
		}
	}
}
